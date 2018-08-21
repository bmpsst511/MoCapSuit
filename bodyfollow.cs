using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class follow : MonoBehaviour {
	public Transform Cam;
    public Transform Body;
	public Transform Head;
	public Transform Neck;
    public Transform Gameobj;
    [SerializeField]
    float CameulerAngX;
    [SerializeField]

    
    float CameulerAngY;
    [SerializeField]
    float CameulerAngZ;
	[SerializeField]
    float CamPositionX;
    [SerializeField]
    float CamPositionY;
    [SerializeField]
    float CamPositionZ;
	[SerializeField]
    float scaleHeight;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //scaleHeight = this.transform.localScale.y;
		//transform.position = InputTracking.GetLocalPosition(XRNode.CenterEye);
        //transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.CenterEye));
		CameulerAngX = transform.localEulerAngles.x;
        CameulerAngY = transform.localEulerAngles.y;
        CameulerAngZ = transform.localEulerAngles.z;
		CamPositionX = transform.localPosition.x;
		CamPositionY = transform.localPosition.y;
		CamPositionZ = transform.localPosition.z;
		Neck.transform.localRotation = Quaternion.Euler(CameulerAngX,CameulerAngY,CameulerAngZ);//正常人體工學，頭轉身體不轉
		Gameobj.transform.position = new Vector3(CamPositionX,CamPositionY,CamPositionZ);//正常人體工學，頭轉身體不轉
        //Body.transform.localRotation = Quaternion.Euler(CameulerAngX,CameulerAngY,CameulerAngZ);//不正常人體工學，頭轉身體轉
		//Gameobj.transform.position = new Vector3(CamPositionX,CamPositionY,CamPositionZ);//不正常人體工學，頭轉身體轉
	}
}
