using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PointerRaycastData : PointerEventData
{
    public GameObject current;
    GameObject lastHower;

    public PointerRaycastData(EventSystem eventSystem) : base(eventSystem)
    {
        pointerId = -10;
        button = InputButton.Left;
    }

   
    void ExecuteHandler<T>(ExecuteEvents.EventFunction<T> handler, GameObject targetOverride = null)
    where T : IEventSystemHandler
    {
        var target = ExecuteEvents.GetEventHandler<T>(targetOverride ? targetOverride : current);
        if (!target) return;

        ExecuteEvents.Execute(target, this, handler);
    }

    public void ExecuteDown()
    {
        eligibleForClick = true;
        clickCount = 1;
        ExecuteHandler(ExecuteEvents.pointerDownHandler);
    }

    public void ExecuteUp()
    {
        ExecuteHandler(ExecuteEvents.pointerUpHandler);
        ExecuteHandler(ExecuteEvents.pointerClickHandler);
        ExecuteSelect();
    }

    public void ExecuteSelect()
    {
        var selectable = current ? current.GetComponentInParent<Selectable>() : null;
        if (selectable && selectable.interactable && selectable.IsActive())
        {
                EventSystem.current.SetSelectedGameObject(current);
                selectable.Select();
        }
        ExecuteHandler(ExecuteEvents.selectHandler, selectable ? selectable.gameObject : current);
    }

    public void ExecuteHover()
    {
        if (lastHower != current)
        {
            if (lastHower != null)
                ExecuteHandler(ExecuteEvents.pointerExitHandler, lastHower);
            if (current != null)
                ExecuteHandler(ExecuteEvents.pointerEnterHandler, current);

            lastHower = current;
            //Logger.Log($"[XRUIRaycast] Hovering {current?.name}");
        }
    }

    public void ExecuteScroll(Vector2 scrollDelta)
    {
        var target = ExecuteEvents.GetEventHandler<IScrollHandler>(current);
        if (!target) return;

        this.scrollDelta = scrollDelta;
        ExecuteHandler(ExecuteEvents.scrollHandler, target);
    }

    public override void Reset()
    {
        base.Reset();
        current = null;
        position = Vector2.zero;
        pressPosition = Vector2.zero;
        
    }
}

public class XRUIRaycast : MonoBehaviour
{
    public Transform ray;
    public float rayDistance = 10f;
    public TMP_Text debugText;
   

    PointerRaycastData ped = new PointerRaycastData(EventSystem.current);
    XRHand hand;

    LineRenderer line;

    private void Awake()
    {
        hand = GetComponentInParent<XRHand>();
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!RaycastUI())
            return;

        if (hand && hand.triggerDown)
        {
            if (debugText && ped.current)
            {
                debugText.text = ped.current.name + "\n" + debugText.text;
            }
            ped.ExecuteDown();
            ped.ExecuteUp();
            //Logger.Log($"[XRUIRaycast] Clicked {ped.current.name}");
        }

        Vector2 scroll = hand ? hand.joystick : Vector2.zero;
        if (scroll != Vector2.zero)
        {
            ped.ExecuteScroll(scroll);
            //Logger.Log($"[XRUIRaycast] Scrolled {ped.current.name} by {scroll}");
        }
    }

    bool RaycastUI()
    {
        var canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.InstanceID);
        if (canvases.Length == 0)
            return false;

        Ray rayCast = new Ray(ray.position, ray.forward);
        ped.Reset();

        foreach (var canvas in canvases)
        {
            if (!canvas.enabled || canvas.renderMode != RenderMode.WorldSpace) continue;

            var plane = new Plane(canvas.transform.forward, canvas.transform.position);
            if (!plane.Raycast(rayCast, out float d)) continue;
            if (d < 0 || d > rayDistance) continue;

            Vector3 wp = rayCast.GetPoint(d);

            var graphics = canvas.GetComponentsInChildren<Graphic>(false);
            if (graphics.Length == 0) continue;

            foreach (var g in graphics)
            {
                if (!g.raycastTarget || !g.gameObject.activeInHierarchy) continue;

                var rt = g.rectTransform;
                Vector2 local = rt.InverseTransformPoint(wp);
                if (rt.rect.Contains(local))
                {
                    //Logger.Log($"[XRUIRaycast] Hit {g.name} on canvas {canvas.name}");

                    RaycastResult rr = new RaycastResult();
                    rr.worldPosition = wp;
                    rr.gameObject = g.gameObject;
                    
                    ped.current = g.gameObject;
                    ped.pointerPressRaycast = rr;
                    ped.pointerCurrentRaycast = rr;                   

                    break;
                }
            }

        }

        ped.ExecuteHover();
        UpdateLine();
        return ped.current != null;
    }   

    void UpdateLine()
    {
        if (line == null) 
            return;

        line.SetPosition(0, new Vector3(0,0,1));
        line.SetPosition(1, ped.current != null ? line.transform.InverseTransformPoint(ped.pointerCurrentRaycast.worldPosition) : Vector3.zero);
    }
}

