using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine.UI;

//引入庫  
using System.Net;  

public class BodyMovement : MonoBehaviour
{
    //以下默認都是私有的成員  
    Socket socket; //目標Socket 
    EndPoint clientEnd; //客户端  
    IPEndPoint ipEnd; //監聽端口  
    public int RotateX, RotateY, RotateZ, RotateX2, RotateY2, RotateZ2;
    public int  AccX,AccY,AccZ,AccX2,AccY2,AccZ2,RecognitionCondition=2;

    public int StraightPunchMax=10, StraightPunchMin=-25, HookMax=45, HookMin=10, UpperMax=130, UpperMin=70;

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

    public Text StraightForce_view;
    public Text HooktForce_view;
    public Text UpperForce_view;


    int score;
 	void InitSocket()  
    {  
        //定義監聽端口，監聽任何IP位址
        ipEnd=new IPEndPoint(IPAddress.Any,27);  
        //定義網路協定  
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);  
        //伺服端绑定ip  
        socket.Bind(ipEnd);  
        //定義客戶端，也是監聽所有客戶端IP  
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
            //清空資料緩存  
            recvData=new byte[1024];  
            //獲取客戶端送來的資料  
            recvLen=socket.ReceiveFrom(recvData,ref clientEnd);  
            //輸出接收到的資料
            recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);  
           // print(recvStr); 
           //分割字串
            char[] splitChar = { ' ', ',', ':', '\t', ';' };
            string[] dataRaw = recvStr.Split(splitChar);
            RotateX = int.Parse(dataRaw[0]);
            RotateY = int.Parse(dataRaw[1]);
            RotateZ = int.Parse(dataRaw[2]);
            RotateX2 = int.Parse(dataRaw[3]);
            RotateY2 = int.Parse(dataRaw[4]);
            RotateZ2 = int.Parse(dataRaw[5]);
            AccX = int.Parse(dataRaw[6]);
            AccY = int.Parse(dataRaw[7]);
            AccZ = int.Parse(dataRaw[8]);
            AccX2 = int.Parse(dataRaw[9]);
            AccY2 = int.Parse(dataRaw[10]);
            AccZ2 = int.Parse(dataRaw[11]);
           // FlexValue = float.Parse(dataRaw[6]);
			// 使用低通濾波
            filter.Step(RotateX);
            filter.Step(RotateY);
            filter.Step(RotateZ);
            filter.Step(RotateX2);
            filter.Step(RotateY2);
            filter.Step(RotateZ2);
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
        LeftForeArm.transform.rotation = Quaternion.Euler(RotateY2, RotateX2-50, -RotateZ2);//cjmcu-055
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
        LFx = RotateX2;
        LFy = RotateY2;
        LFz = RotateZ2;
       //print(LFz+LFy-Lz);
       var Pos1dif = LFz+LFy-Lz;
       var level  = AccX2+AccY2;
       var HookLevel = AccX+AccY2;
       print(level);
       //print(Lz);
       if(AccX>RecognitionCondition){

        if(Pos1dif>StraightPunchMin && Pos1dif<StraightPunchMax)
        {
            if (level>25 )
            {
            string heavy = "大" ;
            StraightForce_view.text="直拳力道:"+heavy;
            print("大");
            }      
            if(level>9 && level<30)
            {
            string weak="小";
            StraightForce_view.text="直拳力道:"+weak;
            }
            string punch1 = "直拳";
            Punch_view.text="拳路:"+punch1;

            print("直拳");
        }
        if(Pos1dif>HookMin && Pos1dif<HookMax)
        {
            if (HookLevel>25 && HookLevel<49 )
            {
            string heavy = "大" ;
            HooktForce_view.text="鉤拳力道:"+heavy;
            print("大");
            }      
            if(HookLevel<18 && HookLevel>10)
            {
            string weak="小";
            HooktForce_view.text="鉤拳力道:"+weak;
            }
            string punch2 = "鉤拳";
            Punch_view.text="拳路:"+punch2;
            print("鉤拳");
        }
        if(Pos1dif>UpperMin && Pos1dif<UpperMax)
        {
             if(AccY2<15 && AccY2>8)
            {
            string weak2="小";
            UpperForce_view.text="上鉤拳力道:"+weak2;
            }
             if(AccY2>20 && AccY2<40)
            {
            string weak2="大";
            UpperForce_view.text="上鉤拳力道:"+weak2;
            }
            string punch3 = "上鉤拳";
            Punch_view.text="拳路:"+punch3;
            print("上鉤拳");
           
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





