using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine.UI;

//引入库  
using System.Net;  

public class BodyMovement : MonoBehaviour
{
   //以下默认都是私有的成员  
    Socket socket; //目標Socket 
    EndPoint clientEnd; //客户端  
    IPEndPoint ipEnd; //監聽端口  
    public float RotateX, RotateY, RotateZ, PoseX2, PoseY2, PoseZ2, AccX,AccY,AccZ;
    string recvStr; //接收的字符串  
    byte[] recvData=new byte[1024]; //接收的資料，必須為字串  
    int recvLen; //接收的字串長度 
    float Lx, Ly, Lz, LFx, LFy, LFz;
    Thread connectThread; //連接執行緒  

    /*新增要控制的部位*/
  /*  public GameObject RightHandIndex1;
    public GameObject RightHandIndex2;
    public GameObject RightHandIndex3;*/
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
    
    public Text Punch_view;

    public Text Force_view;
    public Text Score_view;
    int score;
 	void InitSocket()  
    {  
        //定義監聽端口，監聽任何IP位址
        ipEnd=new IPEndPoint(IPAddress.Any,27);  
        //定義網路協定  
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);  
        //伺服端绑定ip  
        socket.Bind(ipEnd);  
        //定義客戶端，也是監聽所以客戶端IP  
        IPEndPoint sender=new IPEndPoint(IPAddress.Any,0);  
        clientEnd=(EndPoint)sender;  
        print("waiting for UDP dgram");  
  
        //使用新的執行緒執行連線，否則遊戲會卡死  
        connectThread=new Thread(new ThreadStart(SocketReceive));  
        connectThread.Start();  
    }  

	    void SocketReceive()  
    {  
        //接收資料端
        while(true)  
        {  
			var filter = new LowPassFilter(0.95f);
            //清空data緩存  
            recvData=new byte[1024];  
            //獲取客戶端送來的資料  
            recvLen=socket.ReceiveFrom(recvData,ref clientEnd);  
            //输出接收到的資料
            recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);  
           // print(recvStr); 
           //分割字串
            char[] splitChar = { ' ', ',', ':', '\t', ';' };
            string[] dataRaw = recvStr.Split(splitChar);
            RotateX = float.Parse(dataRaw[0]);
            RotateY = float.Parse(dataRaw[1]);
            RotateZ = float.Parse(dataRaw[2]);
            PoseX2 = float.Parse(dataRaw[3]);
            PoseY2 = float.Parse(dataRaw[4]);
            PoseZ2 = float.Parse(dataRaw[5]);
            AccX = float.Parse(dataRaw[6]);
            AccY = float.Parse(dataRaw[7]);
            AccZ = float.Parse(dataRaw[8]);
           // FlexValue = float.Parse(dataRaw[6]);
			// 使用低通濾波
            filter.Step(RotateX);
            filter.Step(RotateY);
            filter.Step(RotateZ);
            filter.Step(PoseX2);
            filter.Step(PoseY2);
            filter.Step(PoseZ2);
            //filter.Step(FlexValue);
           
        }  
    }  

    //關閉連線  
    void SocketQuit()  
    {  
        //關閉執行緒  
        if(connectThread!=null)  
        {  
            connectThread.Interrupt();  
            connectThread.Abort();  
        }  
        //最後關閉Socket 
        if(socket!=null)  
            socket.Close();  
        print("disconnect");  
    } 

    void Start()
    {
       InitSocket(); //初始化Server
    }
    void FixedUpdate()
    {	

        /* 人體骨架控制區 */
        //RightHandIndex1.transform.rotation = Quaternion.Euler(0, 0, -FlexValue);//cjmcu-055
       //RightHandIndex2.transform.rotation = Quaternion.Euler(0, 0, -FlexValue);//cjmcu-055
        //RightHandIndex3.transform.rotation = Quaternion.Slerp(RightHandIndex1.transform.rotation,  RightHandIndex2.transform.rotation, 5*Time.deltaTime);//cjmcu-055

        //Head.transform.rotation = Quaternion.Euler(-RotateX, RotateZ+222, -RotateY);//cjmcu-055

        //LeftShoulder.transform.rotation = Quaternion.Euler(RotateY, RotateZ-50, -RotateX);//cjmcu-055
        LeftForeArm.transform.rotation = Quaternion.Euler(PoseY2, PoseX2-50, -PoseZ2);//cjmcu-055
        LeftArm.transform.rotation = Quaternion.Euler(RotateY, RotateX-58 , -RotateZ);//cjmcu-055
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
        Lx = RotateX;
        Ly = RotateY;
        Lz = RotateZ;
        LFx = PoseX2;
        LFy = PoseY2;
        LFz = PoseZ2;
       //print(LFz+LFy-Lz);
       var Pos1dif = LFz+LFy-Lz;
       /*print(Ly);
       print(Lz);*/
       if(AccX>2){
            string heavy="大";
            string medium="中";
            string weak="小";
        if (AccX>30)
        {Force_view.text="力量"+heavy;}
        if (AccX>10 && AccX<20)
        { Force_view.text="力量"+medium;}
        if(AccX>3 && AccX<5)
        {Force_view.text="力量"+weak;}
        if(Pos1dif>-25 && Pos1dif<10)
        {
            string punch1 = "直拳";
            Punch_view.text="拳路:"+punch1;
            
            print("直拳");
        }
        if(Pos1dif>10 && Pos1dif<45)
        {
            string punch2 = "鉤拳";
            Punch_view.text="拳路:"+punch2;
            print("鉤拳");
        }
       }
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
}





