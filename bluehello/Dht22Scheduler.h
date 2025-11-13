#include <Arduino.h>

class Dht22Scheduler
{
public:
    struct Data
    {
        float humidity = 0;
        float temperature = 0;
        int8_t error = 0; // 0=OK,1=timeout,2=checksum,3=start
        char errorMsg[20] = "OK";
        unsigned long lastUpdate = 0;
        unsigned long lastValidUpdate = 0;
    } data;

    explicit Dht22Scheduler(uint8_t pin, unsigned long intervalMs = 2000) : _pin(pin), _interval(intervalMs) {}

    void begin()
    {
        pinMode(_pin, INPUT_PULLUP);
        data.error = 0;
        strcpy(data.errorMsg, "Not read yet");

        // pinMode(_pin, INPUT_PULLUP);
        //  required delay() for a secure sensor check,
        //  if you reset the mcu very fast one after another
        //  auto tic{millis()};
        //  while ( millis() - tic < READ_FREQUENCY ) {
        //      yield();
        //  }
        //  auto status{read()};
        //  _millis_last_read = millis();
        //  if (status == AM2302_READ_OK) {
        //      return true;
        //  }
        //  else {
        //      return false;
        //  }
    }

    void update()
    {
        unsigned long nowMs = millis();
        if (nowMs - data.lastUpdate >= _interval)
        {
            read_sensor();
            data.lastUpdate = nowMs;
            if (data.error == 0)
            {
                data.lastValidUpdate = nowMs;
            }
        }
    }

private:
    uint8_t _pin;
    unsigned long _interval = 2000;
    unsigned long _lastRead = 0;

    const char *ERROR_TEXTS[5] = {
        "OK",                // 0
        "Read timeout LOW",  // 1
        "Read timeout HIGH", // 2
        "Checksum error",    // 3
        "No response"        // 4
    };
    void read_sensor()
    {
        // *****************************
        //  === send start sequence ===
        // ****************************
        // start from HIGH ==> switch to LOW for min. of 1 ms
        // Set pin to Output
        pinMode(_pin, OUTPUT);
        // set Pin to LOW
        digitalWrite(_pin, LOW);
        // wait min. 1,0 ms
        // delayMicroseconds(1200U);
        waiting(1200U);
        // Set port to HIGH ==> INPUT_PULLUP with PullUp
        digitalWrite(_pin, HIGH);
        pinMode(_pin, INPUT_PULLUP);

        // ******************************
        //  === wait for Acknowledge ===
        // ******************************
        // Acknowledge Sequence 80us LOW 80 us HIGH
        // wait for LOW (80 µs)
        await_state(0);
        // wait for HIGH (80 µs)
        await_state(1);

        // *****************************
        //  ==== Read Sensor Data ====
        // *****************************
        // ==> START of time critical code <==
        // read 40 bits from sensor into the buffer:
        // ==> HIGH state is 70 µs
        // ==> LOW state is 28 µs
        uint8_t _data[5U] = {0};
        if (read_sensor_data(_data, 5U) == 4)
        {
            setError(4);
            return;
        }
        // ==> END of time critical code <==

        // check checksum
        bool _checksum_ok = (_data[4] == ((_data[0] + _data[1] + _data[2] + _data[3]) & 0xFF));

        /*
        // Code part to check the checksum
        // Due to older sensors have an bug an deliver wrong data
        auto d4 = _data[4];
        auto cs = ( (_data[0] + _data[1] + _data[2] + _data[3]) & 0xFF) ;
        Serial.print("delivered Checksum:  ");
        AM2302_Tools::print_byte_as_bit(d4);
        Serial.print("calculated Checksum: ");
        AM2302_Tools::print_byte_as_bit(cs);
        */

        if (!_checksum_ok)
        {
            setError(3);
            return;
        }

        data.humidity = static_cast<uint16_t>((_data[0] << 8) | _data[1]) * 0.1F;
        if (_data[2] & 0x80)
        {
            // negative temperature detected
            _data[2] &= 0x7f;
            data.temperature = -static_cast<int16_t>((_data[2] << 8) | _data[3]) * 0.1F;
        }
        else
        {
            data.temperature = static_cast<int16_t>((_data[2] << 8) | _data[3]) * 0.1F;
        }
        setError(0);
        return;
    }

    int8_t read_sensor_data(uint8_t *buffer, uint8_t size)
    {
        for (uint8_t i = 0; i < size; ++i)
        {
            for (uint8_t bit = 0; bit < 8U; ++bit)
            {
                uint8_t wait_counter{0}, state_counter{0};
                // count wait for state time
                while (!digitalRead(_pin))
                {
                    ++wait_counter;
                    delayMicroseconds(1U);
                    //waiting(1U);
                    if (wait_counter >= 100U)
                    {
                        return 4;
                    }
                }
                // count state time
                while (digitalRead(_pin))
                {
                    ++state_counter;
                    delayMicroseconds(1U);
                    //waiting(1U);
                    if (state_counter >= 100U)
                    {
                        return 4;
                    }
                }
                buffer[i] <<= 1;
                buffer[i] |= (state_counter > wait_counter);                
            }
        }
        return 0;
    }

    int8_t await_state(uint8_t state)
    {
        uint8_t wait_counter{0}, state_counter{0};
        // count wait for state time
        while ((digitalRead(_pin) != state))
        {
            ++wait_counter;
            // delayMicroseconds(1U);
            waiting(1U);
            if (wait_counter >= 100U)
            {
                return 4;
            }
        }
        // count state time
        while ((digitalRead(_pin) == state))
        {
            ++state_counter;
            // delayMicroseconds(1U);
            waiting(1U);
            if (state_counter >= 100U)
            {
                return 4;
            }
        }
        return (state_counter > wait_counter);
    }

    void setError(uint8_t code)
    {
        data.error = code;
        const char *msg = (code < 5) ? ERROR_TEXTS[code] : "Unknown";
        strncpy(data.errorMsg, msg, sizeof(data.errorMsg) - 1);
        data.errorMsg[sizeof(data.errorMsg) - 1] = '\0';
    }
    void waiting(unsigned int time)
    {
        // delayMicroseconds(time);

        auto tic{micros()};
        while (micros() - tic < time)
            yield();
    }
};