﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera {get; private set; }

	// Change to automatically detect camera in scene
	void Awake() {
		m_Camera = Camera.main;
		if (m_Camera == null)
			m_Camera = FindObjectOfType<Camera>();
	}
 
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}

