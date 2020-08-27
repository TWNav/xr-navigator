using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    bool tutorialMode = true;

    /// <summary>
    /// Get or set the <c>ARCameraManager</c>.
    /// </summary>
    public ARCameraManager cameraManager
    {
        get { return m_CameraManager; }
        set
        {
            if (m_CameraManager == value)
                return;

            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;

            m_CameraManager = value;

            if (m_CameraManager != null & enabled)
                m_CameraManager.frameReceived += FrameChanged;
        }
    }

    const string k_FadeOffAnim = "FadeOff";
    const string k_FadeOnAnim = "FadeOn";

    [SerializeField]
    ARPlaneManager m_PlaneManager;

    public ARPlaneManager planeManager
    {
        get { return m_PlaneManager; }
        set { m_PlaneManager = value; }
    }

    [SerializeField]
    Animator m_MoveDeviceAnimation;

    public Animator moveDeviceAnimation
    {
        get { return m_MoveDeviceAnimation; }
        set { m_MoveDeviceAnimation = value; }
    }

    static List<ARPlane> s_Planes = new List<ARPlane>();

    bool m_ShowingMoveDevice = true;

    void OnEnable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += FrameChanged;
    }

    void OnDisable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= FrameChanged;
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (PlanesFound() && m_ShowingMoveDevice)
        {
            if (moveDeviceAnimation)
                moveDeviceAnimation.SetTrigger(k_FadeOffAnim);

            m_ShowingMoveDevice = false;
        }
    }

    bool PlanesFound()
    {
        if (planeManager == null)
            return false;

        return planeManager.trackables.count > 0;
    }


    public void ShowScanEnvironmentAnimation()
    {
        if (moveDeviceAnimation)
        {
            moveDeviceAnimation.SetTrigger(k_FadeOnAnim);
        }
    }

    public void HideScanEnvironmentAnimation()
    {
        if (moveDeviceAnimation)
        {
            moveDeviceAnimation.SetTrigger(k_FadeOffAnim);
        }
    }
}
