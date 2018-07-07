using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
//引入库  
using System.Net;  

public class HandMovement : MonoBehaviour
{
   //以下默认都是私有的成员  
    Socket socket; //目标socket  
    EndPoint clientEnd; //客户端  
    IPEndPoint ipEnd; //侦听端口  
    public float RotateX, RotateY, RotateZ, PoseX2, PoseY2, PoseZ2;
    string recvStr; //接收的字符串  
    string sendStr; //发送的字符串  
    byte[] recvData=new byte[1024]; //接收的数据，必须为字节  
    byte[] sendData=new byte[1024]; //发送的数据，必须为字节  
    int recvLen; //接收的数据长度  
    float Lx, Ly, Lz, LFx, LFy, LFz;
    Thread connectThread; //连接线程  

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

 	void InitSocket()  
    {  
        //定义侦听端口,侦听任何IP  
        ipEnd=new IPEndPoint(IPAddress.Any,27);  
        //定义套接字类型,在主线程中定义  
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);  
        //服务端需要绑定ip  
        socket.Bind(ipEnd);  
        //定义客户端  
        IPEndPoint sender=new IPEndPoint(IPAddress.Any,0);  
        clientEnd=(EndPoint)sender;  
        print("waiting for UDP dgram");  
  
        //开启一个线程连接，必须的，否则主线程卡死  
        connectThread=new Thread(new ThreadStart(SocketReceive));  
        connectThread.Start();  
    }  

/*void SocketSend(string sendStr)  
    {  
        //清空发送缓存  
        sendData=new byte[1024];  
        //数据类型转换  
        sendData=Encoding.ASCII.GetBytes(sendStr);  
        //发送给指定客户端  
        socket.SendTo(sendData,sendData.Length,SocketFlags.None,clientEnd);  
    }*/

	    void SocketReceive()  
    {  
        //进入接收循环  
        while(true)  
        {  
			var filter = new LowPassFilter(0.95f);
            //对data清零  
            recvData=new byte[1024];  
            //获取客户端，获取客户端数据，用引用给客户端赋值  
            recvLen=socket.ReceiveFrom(recvData,ref clientEnd);  
            //print("message from: "+clientEnd.ToString()); //打印客户端信息  
            //输出接收到的数据  
            recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);  
           // print(recvStr); 
            char[] splitChar = { ' ', ',', ':', '\t', ';' };
            string[] dataRaw = recvStr.Split(splitChar);
            RotateX = float.Parse(dataRaw[2]);
            RotateY = float.Parse(dataRaw[1]);
            RotateZ = float.Parse(dataRaw[0]);
            PoseX2 = float.Parse(dataRaw[3]);
            PoseY2 = float.Parse(dataRaw[4]);
            PoseZ2 = float.Parse(dataRaw[5]);
			// Use the `LowPassFilter` to smooth out values
            filter.Step(RotateX);
            filter.Step(RotateY);
            filter.Step(RotateZ);
            filter.Step(PoseX2);
            filter.Step(PoseY2);
            filter.Step(PoseZ2);
           
        }  
    }  

    //连接关闭  
    void SocketQuit()  
    {  
        //关闭线程  
        if(connectThread!=null)  
        {  
            connectThread.Interrupt();  
            connectThread.Abort();  
        }  
        //最后关闭socket  
        if(socket!=null)  
            socket.Close();  
        print("disconnect");  
    } 

    void Start()
    {
       InitSocket(); //在这里初始化server
    }
    void FixedUpdate()
    {
		
        /* Body Control Session */

        //Head.transform.rotation = Quaternion.Euler(-RotateX, RotateZ+222, -RotateY);//cjmcu-055

        //LeftShoulder.transform.rotation = Quaternion.Euler(RotateY, RotateZ-50, -RotateX);//cjmcu-055
        LeftForeArm.transform.rotation = Quaternion.Euler(PoseY2, PoseX2-50, -PoseZ2);//cjmcu-055
        LeftArm.transform.rotation = Quaternion.Euler(RotateY, RotateZ-58 , -RotateX);//cjmcu-055
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
           ball.transform.position = guide.position;

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
         ball.transform.localRotation = transform.rotation;
         //We re-position the ball on our guide object 
         ball.transform.position = guide.position;
 
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





