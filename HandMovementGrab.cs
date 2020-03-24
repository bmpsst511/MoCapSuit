using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
//引入庫  
using System.Net;  

public class HandMovement : MonoBehaviour
{
   //以下默认都是私有的成员  
    Socket socket; //目标socket  
    EndPoint clientEnd; //客户端  
    IPEndPoint ipEnd; //偵聽端口  
    public float RotateX, RotateY, RotateZ, RotateX2, RotateY2, RotateZ2;
    public int Flexval;
    string recvStr; //接收的字符串  
    string sendStr; //發送的字符串  
    byte[] recvData=new byte[1024]; //接收的數據，必須為字節
    byte[] sendData=new byte[1024]; //發送的數據，必須為字節
    int recvLen; //接收的數據長度  
    float Lx, Ly, Lz, LFx, LFy, LFz;
    Thread connectThread; //連接線程  

    //新增要控制的部位
    //public GameObject Head;

    //public GameObject LeftShoulder;
    public GameObject LeftArm;
    public GameObject LeftForeArm;   
    //public GameObject LeftHand;

    //public GameObject RightShoulder;
    //public GameObject RightArm;
    //public GameObject RightForeArm;
    //public GameObject RightHand;

    //public GameObject LeftUpLeg;
    //public GameObject LeftLeg;
    //public GameObject LeftFoot;

    //public GameObject RightUpLeg;
    //public GameObject RightLeg;
    //public GameObject RightFoot;

	 public float speed = 10;
     public bool canHold = true;
     public GameObject ball;
     public Transform guide;

     public GameObject HeadUnity;

     public Animator Anim;
	public AnimatorStateInfo BA;
	public int Grab = Animator.StringToHash("Base Layer.grab2");
	public int Stay = Animator.StringToHash("Base Layer.stayidle2");

 	void InitSocket()  
    {  
        //定義偵聽端口,偵聽任何IP  
        ipEnd=new IPEndPoint(IPAddress.Parse("192.168.1.112"),28);  
        //定義套接字類型,在主線程中定義 
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);  
        //服務端需要綁定ip  
        socket.Bind(ipEnd);  
        //定義客戶端  
        IPEndPoint sender=new IPEndPoint(IPAddress.Parse("192.168.1.116"),28);  
        clientEnd=(EndPoint)sender;  
        print("waiting for UDP dgram");  
  
        //開啟一個線程連接，必須的，否則主線程卡死
        connectThread=new Thread(new ThreadStart(SocketReceive));  
        connectThread.Start();  
    }  

/*void SocketSend(string sendStr)  
    {  
        //清空發送緩存  
        sendData=new byte[1024];  
        //數據類型轉換  
        sendData=Encoding.ASCII.GetBytes(sendStr);  
        //發送給指定客戶端  
        socket.SendTo(sendData,sendData.Length,SocketFlags.None,clientEnd);  
    }*/

	    void SocketReceive()  
    {  
        //進入接收循環  
        while(true)  
        {  
			var filter = new LowPassFilter(0.95f);
            //對data清零  
            recvData=new byte[1024];  
            //獲取客戶端，獲取客戶端數據，用引用給客戶端賦值
            recvLen=socket.ReceiveFrom(recvData,ref clientEnd);  
            //print("message from: "+clientEnd.ToString()); //列印客户端信息
            //輸出接收到的數據  
            recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);  
           // print(recvStr); 
            char[] splitChar = { ' ', ',', ':', '\t', ';' };
            string[] dataRaw = recvStr.Split(splitChar);
            RotateX = float.Parse(dataRaw[0]);
            RotateY = float.Parse(dataRaw[1]);
            RotateZ = float.Parse(dataRaw[2]);
            RotateX2 = float.Parse(dataRaw[3]);
            RotateY2 = float.Parse(dataRaw[4]);
            RotateZ2 = float.Parse(dataRaw[5]);
            Flexval = int.Parse(dataRaw[6]);
			// Use the `LowPassFilter` to smooth out values
            filter.Step(RotateX);
            filter.Step(RotateY);
            filter.Step(RotateZ);
            filter.Step(RotateX2);
            filter.Step(RotateY2);
            filter.Step(RotateZ2);
           
        }  
    }  

    //連接關閉  
    void SocketQuit()  
    {  
        //關閉線程  
        if(connectThread!=null)  
        {  
            connectThread.Interrupt();  
            connectThread.Abort();  
        }  
        //最後關閉socket  
        if(socket!=null)  
            socket.Close();  
        print("disconnect");  
    } 

    void Start()
    {
       InitSocket(); //在這裡初始化server
    }

    void FixedUpdate()
    {
		if(Flexval<-30)
		{
			Anim.SetBool("grabb",true);
            if (!canHold)
               throw_drop();
           else
               Pickup();
		}else
		{
			Anim.SetBool("grabb",false);
		}
    }
    void LateUpdate()
    {

        /* Body Control Session */

        //Head.transform.rotation = Quaternion.Euler(-RotateX, RotateZ+222, -RotateY);//cjmcu-055

        //LeftShoulder.transform.rotation = Quaternion.Euler(RotateY, RotateZ-50, -RotateX);//cjmcu-055
        LeftArm.transform.rotation = Quaternion.Euler(RotateY, RotateX-50 , -RotateZ);//cjmcu-055
        LeftForeArm.transform.rotation = Quaternion.Euler(RotateY2, RotateX2-50, -RotateZ2);//cjmcu-055
        //LeftArm.transform.rotation = Quaternion.Euler(PoseZ2, PoseX2 , PoseY2);//BNO-055
        //LeftHand.transform.rotation = Quaternion.Euler(RotateY, RotateZ-58 , -RotateX);//cjmcu-055

        //RightShoulder.transform.rotation = Quaternion.Euler(-RotateY, RotateZ-198, RotateX);//cjmcu-055
        //RightArm.transform.rotation = Quaternion.Euler(-RotateY, RotateZ-198, RotateX);//cjmcu-055
        //RightForeArm.transform.rotation = Quaternion.Euler(-RotateY, RotateZ-198, RotateX);//cjmcu-055
        //RightHand.transform.rotation = Quaternion.Euler(-RotateY, RotateZ-198, RotateX);//cjmcu-055

        //LeftUpLeg.transform.rotation = Quaternion.Euler(-RotateX, RotateZ-122, -RotateY);//cjmcu-055
        //LeftLeg.transform.rotation = Quaternion.Euler(-RotateX, RotateZ-122, -RotateY);//cjmcu-055
        //LeftFoot.transform.rotation = Quaternion.Euler(-RotateX, RotateZ-122, -RotateY);//cjmcu-055

        //RightUpLeg.transform.rotation = Quaternion.Euler(-RotateX, RotateZ+225, -RotateY);//cjmcu-055
        //RightLeg.transform.rotation = Quaternion.Euler(-RotateX, RotateZ+225, -RotateY);//cjmcu-055
        //RightFoot.transform.rotation = Quaternion.Euler(-RotateX, RotateZ+225, -RotateY);//cjmcu-055
       /* Ly = LeftArm.transform.rotation.y;
        Lx = LeftArm.transform.rotation.x;
        Lz = LeftArm.transform.rotation.z;
        LFx = LeftForeArm.transform.rotation.x;
        LFy = LeftForeArm.transform.rotation.y;
        LFz = LeftForeArm.transform.rotation.z;
       //print(LFy);
        if(Ly>=0.8 && Ly<=0.9 && LFy>=0.98 && LFy<=0.99)
        {
            print("鉤拳");
        }
        if(Ly>=0.6 && Ly<=0.7 && LFy>=0.71 && LFy<=0.8)
        {
            print("直拳");
		}*/

		 if (Input.GetMouseButtonDown(0))
       {
           if (!canHold)
               throw_drop();
           else
               Pickup();
       }//mause If
  
       if (!canHold && ball)
           ball.transform.localPosition = guide.localPosition;

	}

    void OnApplicationQuit()  
    {  
        SocketQuit();  
    } 

    public class LowPassFilter
    {
        private float _smoothingFactor;
        public float SmoothedValue;
        public LowPassFilter(float smoothingFactor)
        {
            _smoothingFactor = smoothingFactor;
        }

        public void Step(float sensorValue)
        {
            SmoothedValue = _smoothingFactor * sensorValue + (1 - _smoothingFactor) * SmoothedValue;
        }
    }

	 //We can use trigger or Collision
     void OnTriggerEnter(Collider col)
     {
         if (col.gameObject.tag == "ball")
             if (!ball) // if we don't have anything holding
                 ball = col.gameObject;
     }

	    //We can use trigger or Collision
     void OnTriggerExit(Collider col)
     {
         if (col.gameObject.tag == "ball")
         {
             if (canHold)
                 ball = null;
         }
     }
 
 
     private void Pickup()
     {
         if (!ball)
             return;
 
         //We set the object parent to our guide empty object.
         ball.transform.SetParent(guide);
 
         //Set gravity to false while holding it
         ball.GetComponent<Rigidbody>().useGravity = false;
 
         //we apply the same rotation our main object (Camera) has.
         ball.transform.localRotation = transform.localRotation;
         //We re-position the ball on our guide object 
         ball.transform.localPosition = guide.localPosition;
 
         canHold = false;
     }
 
     private void throw_drop()
     {
         if (!ball)
             return;
 
         //Set our Gravity to true again.
         ball.GetComponent<Rigidbody>().useGravity = true;
          // we don't have anything to do with our ball field anymore
          ball = null; 
         //Apply velocity on throwing
         guide.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
 
         //Unparent our ball
         guide.GetChild(0).parent = null;
         canHold = true;
     }
}





