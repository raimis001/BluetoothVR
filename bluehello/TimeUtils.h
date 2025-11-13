#include <Arduino.h>

struct TimeParts {
  unsigned long days;
  unsigned int hours;
  unsigned int minutes;
  unsigned int seconds;
};

inline TimeParts millisToTimeParts(unsigned long ms) {
    TimeParts t;
    t.seconds = (ms / 1000UL) % 60UL;
    t.minutes = (ms / 60000UL) % 60UL;
    t.hours   = (ms / 3600000UL) % 24UL;
    t.days    =  ms / 86400000UL;
    return t;
}

inline String formatTimeParts(unsigned long ms) {
    char buffer[32];
    TimeParts t = millisToTimeParts(ms);
    snprintf(buffer, sizeof(buffer), "%lu, %02u:%02u:%02u", t.days, t.hours, t.minutes, t.seconds);
    
    return String(buffer);
}