using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts; //图标插件
using XCharts.Runtime;
using UnityEngine.UI;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Threading;
using HighlightingSystem;
/*
读取的原始数据：
E6 0A 36 B4 0A

温度换算方法
温度 = 0x0AE6 = 2790
湿度 = 0x36 = 54
电量 = 0x0AB4 =2740

实际显示：
温度：27.9 C
湿度：54%
电量: 2.742 V(读数后使用万用表测量电池)
*/
public class MainTargetScript : MonoBehaviour {
    #region UI对象
    public InputField inputIP;//IP地址输入框
    public InputField inputPort;//端口号输入框
    public Button btnConnect;//连接按钮
    public Button btnDisconnect;//断开按钮
    public Text txtConnect;
    public Text txtDisconnect;
    public Toggle togTopic1;//主题选择框
    public Toggle togTopic2;//主题选择框
    public Toggle togTopic3;//主题选择框
    public Toggle togTopic4;//主题选择框
    public Toggle togTopic5;//主题选择框
    public Toggle togTopic6;//主题选择框
    public Toggle togTopic7;//主题选择框
    public Button btnSubscribe;//订阅按钮
    public Text txtResult;//结果输出文本框
    public Dropdown dropTopics;//发布主题选择框
    public InputField inputContent;//发布内容
    public Button btnPublish;//发布按钮
    public GameObject target;
    public Button btnManage; //监测管理按钮
    public Button btnAlarm;  //预警管理按钮
    public Button btnInquire;//多维度查询按钮
    public Button btnAnalyse;//数据统计分析按钮
    public GameObject AlarmCanvas;   //分页面
    public GameObject InquireCanvas;
    public GameObject AnalyseCanvas;
    //AlarmCanvas
    public InputField inputMAXTemp;
    public InputField inputMINTemp;
    public InputField inputMAXWet;
    public InputField inputMINWet;
    public Button Button_AlarTemp;
    //public Button Button_AlarWet;
    public Button Button_AlarFire;
    public Button Button_AlarWater;
    public Button Button_AlarWindow;
    public Text Text_AlarTemp;
    //public Text Text_AlarWet;
    public Text Text_AlarFire;
    public Text Text_AlarWater;
    public Text Text_AlarWindow;
    //数据多维度查询 InquireCanvas
    public LineChart TempChart;    //折线图温度
    public LineChart WetnessChart; //折线图湿度
    public LineChart TempChart1;   //折线图温度分析管理
    public LineChart WetnessChart1; //折线图湿度分析管理
    public BarChart TempBarChart;   //柱状图温度
    public BarChart WetnessBarChart;
    public ScatterChart TempScatterChart;
    public LineChart TempChartA;    //折线图温度
    public LineChart WetnessChartA; //折线图湿度
    public BarChart TempBarChartA;   //柱状图温度
    public BarChart WetnessBarChartA;
    public ScatterChart TempScatterChartA;
    public LineChart TempChartB;    //折线图温度
    public LineChart WetnessChartB; //折线图湿度
    public BarChart TempBarChartB;   //柱状图温度
    public BarChart WetnessBarChartB;
    public ScatterChart TempScatterChartB;
    public Text Text_Fire;
    public Text Text_Water;
    public Text Text_Window;
    public Text Text_Window0;
    public Text Text_Temp0;     //预报警效果
    public Text Text_Temp1;
    public Text Text_Temp2;
    public Text Text_Wet0;
    public Text Text_Wet1;
    public Text Text_Wet2;
    //数据统计分析部分  AnalyseCanvas

    public Text Text_Fire1;     
    public Text Text_Water1;
    public Text Text_Window1;
    public Text Text_Window11;
    public Text Text_MinTemp;
    public Text Text_MaxTemp;
    public Text Text_AveTemp;
    public Text Text_MinTempA;
    public Text Text_MaxTempA;
    public Text Text_AveTempA;
    public Text Text_MinTempB;
    public Text Text_MaxTempB;
    public Text Text_AveTempB;
    public Text Text_MinWet;
    public Text Text_MaxWet;
    public Text Text_AveWet;
    public Text Text_MinWetA;
    public Text Text_MaxWetA;
    public Text Text_AveWetA;
    public Text Text_MinWetB;
    public Text Text_MaxWetB;
    public Text Text_AveWetB;
    //世界空间物体
    public GameObject box1;
    public GameObject panel;
    public GameObject panel1;
    public GameObject server4;
    public GameObject serverL;
    public GameObject serverM;
    //箱子面板
    public GameObject server4Canvas;
    public Button btnS4Close;
    public Text S4txtResult;
    public GameObject serverMCanvas;
    public Button btnMClose;
    public Text MtxtResult;
    public GameObject serverLCanvas;
    public Button btnLClose;
    public Text LtxtResult;
    //门窗 0 1
    public GameObject PanelCanvas;
    public GameObject PanelCanvas1;
    public Button btnPClose;
    public Button btnP1Close;
    public Text PtxtResult;
    public Text P1txtResult;
    //水浸 火警
    public GameObject BoxCanvas1;
    public Button btnB1Close;
    public Text B1txtResult;
    #endregion

    #region 变量
    private string[] allTopics = { "APP", "first", "second", "third", "forth", "fifth", "sixth", "seventh" };//发布的名字
    private List<string> selectedTopics;//选中订阅的主题
    private string currentTopic;        //选中发布的主题
    private MqttClient client;          //客户端
    string tmp = "";                    //初始化获取到的数据
    public double temperature = 0;             //初始化温度
    public double temperatureA = 0;            //模拟数据一
    public double temperatureB = 0;

    ArrayList Maxtemperature = new ArrayList();//温度存储表
    ArrayList MaxtemperatureA = new ArrayList();//模拟数据一存储表
    ArrayList MaxtemperatureB = new ArrayList();
    ArrayList MaxWet = new ArrayList();        //shi度存储表
    ArrayList MaxWetA = new ArrayList();        //shi度存储表
    ArrayList MaxWetB = new ArrayList();        //shi度存储表
    MyCompare myCompare = new MyCompare();//创建自定义比较器实例
    double Avetemperature = 0;            //平均温度
    double AvetemperatureA = 0;
    double AvetemperatureB = 0;
    double AveWet = 0;                    //平均湿度
    double AveWetA = 0;
    double AveWetB = 0;
    //AlarmCanvas
    int AlarTemp = 0;
    //int AlarWet = 0;
    int AlarFire = 0;
    int AlarWater = 0;
    int AlarWindow = 0;

    int wetness = 0;                    //初始化湿度
    int wetnessA = 0;
    int wetnessB = 0;
    double electricity = 0;             //初始化电量
    double electricityA = 0;
    double electricityB = 0;
    int fire = 0;
    int water = 0;
    int window1 = 0;
    int window2 = 0;
    private bool isChanged = false;     //判断接收的总数据是否改变了
    private bool isChanged1 = false;    ////判断接收的温湿度数据是否改变了
    private bool isChangedA = false;
    private bool isChangedB = false;
    int Connect1 = 0;
    #endregion

    // Use this for initialization
    void Start () {
        //主题
        selectedTopics = new List<string> {  };//订阅的       
        btnConnect.onClick.AddListener(btnConnect_Click);        //连接断开
        btnDisconnect.onClick.AddListener(btnDisconnect_Click);
        btnSubscribe.onClick.AddListener(btnSubscribe_Click);    //订阅发布
        btnPublish.onClick.AddListener(btnPublish_Click);
        dropTopics.onValueChanged.AddListener(dropTopics_ValueChange);  //发布主题  下拉选择项函数
        togTopic1.onValueChanged.AddListener(togTopic_ValueChange);     //上边四个选择项
        togTopic2.onValueChanged.AddListener(togTopic_ValueChange);
        togTopic3.onValueChanged.AddListener(togTopic_ValueChange);
        togTopic4.onValueChanged.AddListener(togTopic_ValueChange);
        togTopic5.onValueChanged.AddListener(togTopic_ValueChange);
        togTopic6.onValueChanged.AddListener(togTopic_ValueChange);
        togTopic7.onValueChanged.AddListener(togTopic_ValueChange);
        //
        btnManage.onClick.AddListener(BtnManage);             // 下边四个按钮                        监测管理按钮点击事件
        btnAlarm.onClick.AddListener(BtnAlarm);
        btnInquire.onClick.AddListener(BtnInquire);
        btnAnalyse.onClick.AddListener(BtnAnalyse);
        AlarmCanvas.SendMessage("Hide"); //画布组
        InquireCanvas.SendMessage("Hide");
        AnalyseCanvas.SendMessage("Hide");
        //TempChart.RemoveData();          //清除图表数据
        TempChart.AddSerie<Line>("line");
        TempChart.AddSerie<Line>("line");
        TempBarChart.AddSerie<Line>("line");
        WetnessBarChart.AddSerie<Line>("line");
        TempScatterChart.AddSerie<Line>("line");
        // WetnessScatterChart.AddSerie<Line>("line");fg\cxxxxx
        //
        btnS4Close.onClick.AddListener(BtnS4Close);
        btnPClose.onClick.AddListener(BtnPClose);
        btnP1Close.onClick.AddListener(BtnP1Close);
        btnB1Close.onClick.AddListener(BtnB1Close);
        btnMClose.onClick.AddListener(BtnMClose);
        btnLClose.onClick.AddListener(BtnLClose);
        server4Canvas.SendMessage("Hide");
        PanelCanvas.SendMessage("Hide");
        PanelCanvas1.SendMessage("Hide");
        BoxCanvas1.SendMessage("Hide");
        serverMCanvas.SendMessage("Hide");
        serverLCanvas.SendMessage("Hide");
        //AlarmCanvas
        Button_AlarTemp.onClick.AddListener(ButAlarTemp);
        //Button_AlarWet.onClick.AddListener(ButAlarWet);
        Button_AlarFire.onClick.AddListener(ButAlarFire);
        Button_AlarWater.onClick.AddListener(ButAlarWater);
        Button_AlarWindow.onClick.AddListener(ButAlarWindow);
    }
    #region 
    //订阅主题选中事件  上边四个选择项
    private void togTopic_ValueChange(bool arg0)
    {
        //获取当前选中的对象
        var current = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        string topic = current.GetComponentInChildren<Text>().text;
        if (arg0)//选中
        {
            selectedTopics.Add(topic);      //topic是文本的字是啥
            Debug.Log("当前选中的订阅对象：" + topic);
        }
        else//未选中
        {
            selectedTopics.Remove(topic);
        }
    } 
    #endregion

    #region 发布主题选中事件
    private void dropTopics_ValueChange(int arg0)
    {
        currentTopic = allTopics[arg0];
    } 
    #endregion

    // Update is called once per frame
    void Update () {
		
	}

    #region 发布按钮点击事件
    private void btnPublish_Click()
    {
        string content = inputContent.text;
        if (client!=null&&!string.IsNullOrEmpty(currentTopic)&&!string.IsNullOrEmpty(content))
        {
            client.Publish(currentTopic, System.Text.Encoding.UTF8.GetBytes(content), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("发布选项点击成功："+ currentTopic + "内容为" + content);
        }
    } 
    #endregion

    #region 订阅按钮点击事件
    private void btnSubscribe_Click()
    {
        if (client!=null&&selectedTopics!=null)
        {
            Debug.Log(selectedTopics.Count);
            for (int i = 0; i< selectedTopics.Count ; i++) {
                Debug.Log( "订阅的主题为" + selectedTopics[i]);
                client.Subscribe(new string[] { selectedTopics[i] }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });//初始化数组和赋值
              //  client.Subscribe(new string[] { "second" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });//初始化数组和赋值
            }
        }
    } 
    #endregion

    #region 断开按钮点击事件
    private void btnDisconnect_Click()
    {
        if (client!=null)
        {
            client.Disconnect();
            Connect1 = 0;
        }
    } 
    #endregion

    #region 连接按钮点击事件
    private void btnConnect_Click()
    {
        string txtIP = inputIP.text;                      //ip地址
        string txtPort = inputPort.text;                  //端口号
        string clientId = "电脑Computer"+Guid.NewGuid().ToString();   //名字
        string username = "admin";
        string password = "admin";
        client = new MqttClient(IPAddress.Parse(txtIP), int.Parse(txtPort),false,null);
        //客户端实例化    接收
        client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        //绑定接收事件    订阅
        client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
        //连接服务端
        client.Connect(clientId, username, password);
        Debug.Log("连接成功，clientId为：" + clientId);   
        Connect1 = 1;
    }
    
    private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
    {
        Debug.Log("Subscribe订阅成功"+e.MessageId);   //订阅
    }
    //接收到发布的消息
    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        Debug.Log("接收到主题Topic:"+e.Topic);  //控制台输出
        tmp = System.Text.Encoding.UTF8.GetString(e.Message);    //字符串 接收到的数据
        Debug.Log("接收到消息Message:" + tmp);
        switch (e.Topic)
        {
            case "first":
                //字符数组  温度
                isChanged1 = true;
                string[] temperature0 = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    temperature0[i] = tmp.Substring(i, 1);
                    Console.WriteLine(temperature0[i]);
                  //Debug.Log("接收到温度" + temperature0[i]);
                }                
                int temperature1 = int.Parse(temperature0[2], System.Globalization.NumberStyles.HexNumber) * 16 * 16 * 16;
                int temperature2 = int.Parse(temperature0[3], System.Globalization.NumberStyles.HexNumber) * 16 * 16;
                int temperature3 = int.Parse(temperature0[0], System.Globalization.NumberStyles.HexNumber)  * 16;
                int temperature4 = int.Parse(temperature0[1], System.Globalization.NumberStyles.HexNumber) ;
                temperature = Convert.ToDouble( (temperature1 + temperature2 + temperature3 + temperature4) )/ 100;                
                Maxtemperature.Add(Convert.ToString(temperature));                         //数据存储 //qiu最大最小温度
                if (Maxtemperature.Count > 1) {                                            //如果不只有一个数据,则比较
                    Maxtemperature.Sort(myCompare);                     
                    //Debug.Log("排序后: " + Maxtemperature);
                }
                Avetemperature = 0;
                foreach (var v in Maxtemperature)// 平均温度    将 forEach遍历，求和
                {
                    Avetemperature = Avetemperature + Convert.ToDouble(v);
                }
                    Avetemperature = Avetemperature / Maxtemperature.Count;                
                //Debug.Log("最大温度Mintemperature:(°) " + Maxtemperature[0]+ "平均温度" + Avetemperature);
                //Debug.Log("温度:(°) " + temperature.ToString("F1"));    //表示想要展示一位小数
                //字符数组  湿度
                string[] wetness0 = new string[2];
                for (int i = 0; i < 2; i++)
                {
                    wetness0[i] = tmp.Substring(i + 4, 1);
                    Console.WriteLine(wetness0[i]);
                    //Debug.Log("接收到湿度" + wetness0[i]);
                }
                wetness = int.Parse(wetness0[0], System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(wetness0[1], System.Globalization.NumberStyles.HexNumber);
                MaxWet.Add(Convert.ToString(wetness));                       
                if (MaxWet.Count > 1)
                {                         
                    MaxWet.Sort(myCompare);                       
                }
                AveWet = 0;
                foreach (var v in MaxWet)
                {
                    AveWet = AveWet + Convert.ToDouble(v);
                }
                AveWet = AveWet / MaxWet.Count;
                //字符数组  电量
                string[] electricity0 = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    electricity0[i] = tmp.Substring(i+6, 1);
                    Console.WriteLine(electricity0[i]);
                }
                int electricity1 = int.Parse(electricity0[2], System.Globalization.NumberStyles.HexNumber) * 16 * 16 * 16;
                int electricity2 = int.Parse(electricity0[3], System.Globalization.NumberStyles.HexNumber) * 16 * 16;
                int electricity3 = int.Parse(electricity0[0], System.Globalization.NumberStyles.HexNumber) * 16;
                int electricity4 = int.Parse(electricity0[1], System.Globalization.NumberStyles.HexNumber);
                electricity = Convert.ToDouble((electricity1 + electricity2 + electricity3 + electricity4)) / 1000;
                Debug.Log("温度:(°) " + temperature.ToString("F1")+" 湿度:(%) " + wetness+" 电量(V): " + electricity.ToString("F3"));    //表示想要展示一位小数
                break;
            case "second"://火警
                string[] fire0 = new string[1];
                fire0[0] = tmp.Substring(0, 1);
                Console.WriteLine(fire0[0]);
                fire = int.Parse(fire0[0]) ;               
                break;
            case "third"://水浸
                string[] Water0 = new string[1];
                Water0[0] = tmp.Substring(0, 1);
                Console.WriteLine(Water0[0]);
                water = int.Parse(Water0[0]);                
                break;
            case "forth"://门窗
                string[] Window0 = new string[1];
                Window0[0] = tmp.Substring(0, 1);
                Console.WriteLine(Window0[0]);
                window1 = int.Parse(Window0[0]);                
                break;
            case "fifth"://门窗二
                string[] Window00 = new string[1];
                Window00[0] = tmp.Substring(0, 1);
                Console.WriteLine(Window00[0]);
                window2 = int.Parse(Window00[0]);
                break;
            case "sixth"://模拟一
                         //字符数组  温度
                isChangedA = true;
                string[] temperature00 = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    temperature00[i] = tmp.Substring(i, 1);
                    Console.WriteLine(temperature00[i]);
                }
                int temperature11 = int.Parse(temperature00[2], System.Globalization.NumberStyles.HexNumber) * 16 * 16 * 16;
                int temperature22 = int.Parse(temperature00[3], System.Globalization.NumberStyles.HexNumber) * 16 * 16;
                int temperature33 = int.Parse(temperature00[0], System.Globalization.NumberStyles.HexNumber) * 16;
                int temperature44 = int.Parse(temperature00[1], System.Globalization.NumberStyles.HexNumber);
                temperatureA = Convert.ToDouble((temperature11 + temperature22 + temperature33 + temperature44)) / 100;
                MaxtemperatureA.Add(Convert.ToString(temperatureA));                         //数据存储 qiu最大最小温度
                if (MaxtemperatureA.Count > 1)
                {                                            //不只有一个数据则比较
                    MaxtemperatureA.Sort(myCompare);
                }
                AvetemperatureA = 0;
                foreach (var v in MaxtemperatureA)// 平均温度    将 forEach遍历，求和
                {
                    AvetemperatureA = AvetemperatureA + Convert.ToDouble(v);
                }
                AvetemperatureA = AvetemperatureA / MaxtemperatureA.Count;
                //Debug.Log("最大温度MintemperatureA:(°) " + MaxtemperatureA[0] + "平均温度A" + AvetemperatureA);
                //字符数组  湿度
                string[] wetness00 = new string[2];
                for (int i = 0; i < 2; i++)
                {
                    wetness00[i] = tmp.Substring(i + 4, 1);
                    Console.WriteLine(wetness00[i]);
                }
                wetnessA = int.Parse(wetness00[0], System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(wetness00[1], System.Globalization.NumberStyles.HexNumber);
                MaxWetA.Add(Convert.ToString(wetnessA));
                if (MaxWetA.Count > 1)
                {
                    MaxWetA.Sort(myCompare);
                }
                AveWetA = 0;
                foreach (var v in MaxWetA)
                {
                    AveWetA = AveWetA + Convert.ToDouble(v);
                }
                AveWetA = AveWetA / MaxWetA.Count;
                //Debug.Log("湿度A:(%) " + wetnessA);
                //字符数组  电量
                string[] electricity00 = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    electricity00[i] = tmp.Substring(i + 6, 1);
                    Console.WriteLine(electricity00[i]);
                }
                int electricity11 = int.Parse(electricity00[2], System.Globalization.NumberStyles.HexNumber) * 16 * 16 * 16;
                int electricity22 = int.Parse(electricity00[3], System.Globalization.NumberStyles.HexNumber) * 16 * 16;
                int electricity33 = int.Parse(electricity00[0], System.Globalization.NumberStyles.HexNumber) * 16;
                int electricity44 = int.Parse(electricity00[1], System.Globalization.NumberStyles.HexNumber);
                electricityA = Convert.ToDouble((electricity11 + electricity22 + electricity33 + electricity44)) / 1000;
                Debug.Log("温度A:(°) " + temperatureA.ToString("F1")+"湿度A:(%) " + wetnessA+" 电量(V): " + electricityA.ToString("F3"));    //表示想要展示一位小数
                break;
            case "seventh"://模拟二
                           //字符数组  温度
                isChangedB = true;
                string[] temperature000 = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    temperature000[i] = tmp.Substring(i, 1);
                    Console.WriteLine(temperature000[i]);
                }
                int temperature111 = int.Parse(temperature000[2], System.Globalization.NumberStyles.HexNumber) * 16 * 16 * 16;
                int temperature222 = int.Parse(temperature000[3], System.Globalization.NumberStyles.HexNumber) * 16 * 16;
                int temperature333 = int.Parse(temperature000[0], System.Globalization.NumberStyles.HexNumber) * 16;
                int temperature444 = int.Parse(temperature000[1], System.Globalization.NumberStyles.HexNumber);
                temperatureB = Convert.ToDouble((temperature111 + temperature222 + temperature333 + temperature444)) / 100;
                MaxtemperatureB.Add(Convert.ToString(temperatureB));                         //数据存储 qiu最大最小温度
                if (MaxtemperatureB.Count > 1)
                {                                            //不只有一个数据则比较
                    MaxtemperatureB.Sort(myCompare);
                }
                AvetemperatureB = 0;
                foreach (var v in MaxtemperatureB)// 平均温度    将 forEach遍历，求和
                {
                    AvetemperatureB = AvetemperatureB + Convert.ToDouble(v);
                }
                AvetemperatureB = AvetemperatureB / MaxtemperatureB.Count;
                //Debug.Log("最大温度MintemperatureB:(°) " + MaxtemperatureB[0] + "平均温度B" + AvetemperatureB);
                //字符数组  湿度
                string[] wetness000 = new string[2];
                for (int i = 0; i < 2; i++)
                {
                    wetness000[i] = tmp.Substring(i + 4, 1);
                    Console.WriteLine(wetness000[i]);
                }
                wetnessB = int.Parse(wetness000[0], System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(wetness000[1], System.Globalization.NumberStyles.HexNumber);
                MaxWetB.Add(Convert.ToString(wetnessB));
                if (MaxWetB.Count > 1)
                {
                    MaxWetB.Sort(myCompare);
                }
                AveWetB = 0;
                foreach (var v in MaxWetB)
                {
                    AveWetB = AveWetB + Convert.ToDouble(v);
                }
                AveWetB = AveWetB / MaxWetB.Count;
                //Debug.Log("湿度A:(%) " + wetnessA);
                //字符数组  电量
                string[] electricity000 = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    electricity000[i] = tmp.Substring(i + 6, 1);
                    Console.WriteLine(electricity000[i]);
                }
                int electricity111 = int.Parse(electricity000[2], System.Globalization.NumberStyles.HexNumber) * 16 * 16 * 16;
                int electricity222 = int.Parse(electricity000[3], System.Globalization.NumberStyles.HexNumber) * 16 * 16;
                int electricity333 = int.Parse(electricity000[0], System.Globalization.NumberStyles.HexNumber) * 16;
                int electricity444 = int.Parse(electricity000[1], System.Globalization.NumberStyles.HexNumber);
                electricityB = Convert.ToDouble((electricity111 + electricity222 + electricity333 + electricity444)) / 1000;
                Debug.Log("温度B:(°) " + temperatureB.ToString("F1") + "湿度B:(%) " + wetnessB + " 电量(V): " + electricityB.ToString("F3"));    //表示想要展示一位小数
                break;
        }      
        isChanged = true; 
    }
    int i,iA,iB = 10;
    double[] TempArray = { 0, 0, 0, 0, 0,0,0,0,0,0 };
    double[] WetArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    double[] TempArrayA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    double[] WetArrayA = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    double[] TempArrayB = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    double[] WetArrayB = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private void OnGUI()
    {
        if (Connect1 == 1)
        {
            txtConnect.text = "已连接";
            txtDisconnect.text = "断开";
        }
        else
        {
            txtConnect.text = "连接";
            txtDisconnect.text = "已断开";
        }
           
        //AlarmCanvas的开关控制(无mqtt数据接收时)
        if (AlarTemp == 1)
            Text_AlarTemp.text = "开关：开";
        else
        {
            Text_AlarTemp.text = "开关：关";
        }
        if (AlarFire == 1)
        {
            Text_AlarFire.text = "开关：开";
            //Text_AlarFire.color = Color.black;
        }
        else
        {
            Text_AlarFire.text = "开关：关";
            Text_Fire.text = "火警：无";
            Text_Fire.color = Color.black;
            Text_Fire1.text = "火警：无";
            Text_Fire1.color = Color.black;
            B1txtResult.text = "火警：无";
        }
        if (AlarWater == 1)
            Text_AlarWater.text = "开关：开";       
        else
        {
            Text_AlarWater.text = "开关：关";
            Text_Water.text = "水浸：无";
            Text_Water1.text = "水浸：无";
            B1txtResult.text += "\n水浸：无";
        }
        if (AlarWindow == 1)
            Text_AlarWindow.text = "开关：开";           
        else
        {
            Text_AlarWindow.text = "开关：关";
            Text_Window.text = "门窗：无";
            Text_Window0.text = "门窗：无";
            Text_Window1.text = "门窗：无";
            Text_Window11.text = "门窗：无";
            PtxtResult.text = "门窗：无";
            P1txtResult.text = "门窗：无";
        }
        //有mqtt数据接收时
        if (isChanged) {
            //当接收到温湿度的变化时
            if (isChanged1)
            {
                if (AlarTemp == 1)
                {
                    Text_AlarTemp.text = "开关：开";
                    txtResult.text += "\n";
                    txtResult.text += tmp;
                    txtResult.text += " 温度(°):";
                    txtResult.text += temperature;
                    txtResult.text += " 湿度(%):";
                    txtResult.text += wetness;
                    txtResult.text += " 电量(V):";
                    txtResult.text += electricity;
                    S4txtResult.text = "温度(°):" + temperature + " \n湿度(%):" + wetness + " \n电压(V):"+ electricity;
                    //AnalyseCanvas
                    Text_MinTemp.text = "最低温度(°):" + Maxtemperature[0];
                    Text_MaxTemp.text = "最高温度(°):" + Maxtemperature[Maxtemperature.Count - 1];
                    Text_AveTemp.text = "平均温度(°):" + Avetemperature.ToString("F1");
                    Text_MinWet.text = "最低湿度(% ):" + MaxWet[0];
                    Text_MaxWet.text = "最高湿度(% ):" + MaxWet[MaxWet.Count - 1];
                    Text_AveWet.text = "平均湿度(% ):" + AveWet.ToString("F1");
                    //温湿度 图只统计最近十个数(顺序从后往前)
                  
                        for (int j = 0; j < 9; j++)
                        {
                            TempArray[j] = TempArray[j + 1];
                            TempChart.UpdateData(0, j, TempArray[j + 1]);
                            //TempChart1.UpdateData(0, j, TempArray[j + 1]);
                            TempBarChart.UpdateData(0, j, TempArray[j + 1]);
                            // TempScatterChart.UpdateData(0, j, TempArray[j + 1]);                   

                            WetArray[j] = WetArray[j + 1];
                            WetnessChart.UpdateData(0, j, WetArray[j + 1]);
                            //WetnessChart1.UpdateData(0, j, WetArray[j + 1]);
                            WetnessBarChart.UpdateData(0, j, WetArray[j + 1]);
                            TempScatterChart.UpdateData(0, j, 1, TempArray[j + 1]);   //散点图
                            TempScatterChart.UpdateData(0, j, 0, WetArray[j + 1]);
                            // TempChart.AddXAxisData("X"+(i%10));
                            // TempChart.AddData(i%10,temperature);
                        }
                        //折线图
                        TempArray[9] = temperature;
                        TempChart.UpdateData(0, 9, temperature);
                        //TempChart1.UpdateData(0, 9, temperature);
                        WetArray[9] = wetness;
                        WetnessChart.UpdateData(0, 9, wetness);
                        //WetnessChart1.UpdateData(0, 9, wetness);
                        //柱状图
                        TempBarChart.UpdateData(0, 9, temperature);
                        WetnessBarChart.UpdateData(0, 9, wetness);
                        //散点图
                        TempScatterChart.UpdateData(0, 9, 1, temperature);
                        TempScatterChart.UpdateData(0, 9, 0, wetness);
                        //WetnessScatterChart.UpdateData(0, 9, wetness);
                        i = i + 1;                    
                }
                else
                {
                    Text_AlarTemp.text = "开关：关";
                }
                isChanged1 = false;
            }
            //当接收到温湿度A的变化时
            if (isChangedA)
            {
                if (AlarTemp == 1)
                {
                    Text_AlarTemp.text = "开关：开";
                    txtResult.text += "\n";
                    txtResult.text += tmp;
                    txtResult.text += " 温度A(°):";
                    txtResult.text += temperatureA;
                    txtResult.text += " 湿度A(%):";
                    txtResult.text += wetnessA;
                    txtResult.text += " 电量A(V):";
                    txtResult.text += electricityA;                    
                    MtxtResult.text = "温度A(°):" + temperatureA + " \n湿度A(%):" + wetnessA + " \n电压A(V):"+ electricityA;
                    //AnalyseCanvas
                    Text_MinTempA.text = "最低温度A(°):" + MaxtemperatureA[0];
                    Text_MaxTempA.text = "最高温度A(°):" + MaxtemperatureA[MaxtemperatureA.Count - 1];
                    Text_AveTempA.text = "平均温度A(°):" + AvetemperatureA.ToString("F1");
                    Text_MinWetA.text = "最低湿度A(% ):" + MaxWetA[0];
                    Text_MaxWetA.text = "最高湿度A(% ):" + MaxWetA[MaxWetA.Count - 1];
                    Text_AveWetA.text = "平均湿度A(% ):" + AveWetA.ToString("F1");
                   
                    //温湿度 图只统计最近十个数(顺序从后往前)               
                        for (int j = 0; j < 9; j++)
                        {
                            TempArrayA[j] = TempArrayA[j + 1];
                            TempChartA.UpdateData(0, j, TempArrayA[j + 1]);
                            //TempChart1A.UpdateData(0, j, TempArrayA[j + 1]);   //分析界面的
                            TempBarChartA.UpdateData(0, j, TempArrayA[j + 1]);
                   
                            WetArrayA[j] = WetArrayA[j + 1];
                            WetnessChartA.UpdateData(0, j, WetArrayA[j + 1]);
                        //WetnessChart1A.UpdateData(0, j, WetArrayA[j + 1]);  //分析界面的
                            WetnessBarChartA.UpdateData(0, j, WetArrayA[j + 1]);
                            TempScatterChartA.UpdateData(0, j, 1, TempArrayA[j + 1]);   //散点图
                            TempScatterChartA.UpdateData(0, j, 0, WetArrayA[j + 1]);
                        }
                        //折线图
                        TempArrayA[9] = temperatureA;
                        TempChartA.UpdateData(0, 9, temperatureA);
                        //TempChart1A.UpdateData(0, 9, temperatureA);
                        WetArrayA[9] = wetnessA;
                        WetnessChartA.UpdateData(0, 9, wetnessA);
                       // WetnessChart1A.UpdateData(0, 9, wetnessA);
                        //柱状图
                        TempBarChartA.UpdateData(0, 9, temperatureA);
                        WetnessBarChartA.UpdateData(0, 9, wetnessA);
                        //散点图
                        TempScatterChartA.UpdateData(0, 9, 1, temperatureA);
                        TempScatterChartA.UpdateData(0, 9, 0, wetnessA);                        
                        iA = iA + 1;                  
                }
                else
                {
                    Text_AlarTemp.text = "开关：关";
                }
                isChangedA = false;
            }
            //当接收到温湿度B的变化时
            if (isChangedB)
            {
                if (AlarTemp == 1)
                {
                    Text_AlarTemp.text = "开关：开";
                    txtResult.text += "\n";
                    txtResult.text += tmp;
                    txtResult.text += " 温度B(°):";
                    txtResult.text += temperatureB;
                    txtResult.text += " 湿度B(%):";
                    txtResult.text += wetnessB;
                    txtResult.text += " 电量B(V):";
                    txtResult.text += electricityB;
                    LtxtResult.text = "温度B(°):" + temperatureB + " \n湿度B(%):" + wetnessB + " \n电压B(V):"+ electricityB;
                    //AnalyseCanvas
                       Text_MinTempB.text = "最低温度B(°):" + MaxtemperatureB[0];
                       Text_MaxTempB.text = "最高温度B(°):" + MaxtemperatureB[MaxtemperatureB.Count - 1];
                       Text_AveTempB.text = "平均温度B(°):" + AvetemperatureB.ToString("F1");
                       Text_MinWetB.text = "最低湿度B(% ):" + MaxWetB[0];
                       Text_MaxWetB.text = "最高湿度B(% ):" + MaxWetB[MaxWetB.Count - 1];
                       Text_AveWetB.text = "平均湿度B(% ):" + AveWetB.ToString("F1");
                      
                    //温湿度 图只统计最近十个数(顺序从后往前)
                    for (int j = 0; j < 9; j++)
                    {
                        TempArrayB[j] = TempArrayB[j + 1];
                        TempChartB.UpdateData(0, j, TempArrayB[j + 1]);
                        //TempChart1B.UpdateData(0, j, TempArrayB[j + 1]);   //分析界面的
                        TempBarChartB.UpdateData(0, j, TempArrayB[j + 1]);

                        WetArrayB[j] = WetArrayB[j + 1];
                        WetnessChartB.UpdateData(0, j, WetArrayB[j + 1]);
                        //WetnessChart1B.UpdateData(0, j, WetArrayB[j + 1]);  //分析界面的
                        WetnessBarChartB.UpdateData(0, j, WetArrayB[j + 1]);
                        TempScatterChartB.UpdateData(0, j, 1, TempArrayB[j + 1]);   //散点图
                        TempScatterChartB.UpdateData(0, j, 0, WetArrayB[j + 1]);
                    }
                    //折线图
                    TempArrayB[9] = temperatureB;
                    TempChartB.UpdateData(0, 9, temperatureB);
                    //TempChart1B.UpdateData(0, 9, temperatureB);
                    WetArrayB[9] = wetnessB;
                    WetnessChartB.UpdateData(0, 9, wetnessB);
                    // WetnessChart1B.UpdateData(0, 9, wetnessB);
                    //柱状图
                    TempBarChartB.UpdateData(0, 9, temperatureB);
                    WetnessBarChartB.UpdateData(0, 9, wetnessB);
                    //散点图
                    TempScatterChartB.UpdateData(0, 9, 1, temperatureB);
                    TempScatterChartB.UpdateData(0, 9, 0, wetnessB);
                    iB = iB + 1;
                }
                else
                {
                    Text_AlarTemp.text = "开关：关";
                }
                isChangedB = false;
            }
            //温度预警
            if (AlarTemp == 1)
            {
                string txtMAXTemp = inputMAXTemp.text;
                string txtMINTemp = inputMINTemp.text;
                string txtMAXWet = inputMAXWet.text;
                string txtMINWet = inputMINWet.text;
                int txtMAX1 = int.Parse(txtMAXTemp);  //取整数
                int txtMIN1 = int.Parse(txtMINTemp);
                int txtMAX2 = int.Parse(txtMAXWet);  
                int txtMIN2 = int.Parse(txtMINWet);
                Debug.Log("最大温度哈哈:(°) " + txtMAX1 + "最小温度:(%) " + txtMIN1);
                Debug.Log("最大湿度哈哈:(°) " + txtMAX2 + "最小湿度:(%) " + txtMIN2);
                if (AlarmCanvas.transform.GetComponent<CanvasGroup>().alpha == 0 && InquireCanvas.transform.GetComponent<CanvasGroup>().alpha == 0 && AnalyseCanvas.transform.GetComponent<CanvasGroup>().alpha == 0)
                {
                    if (temperature >= txtMAX1 || temperature <= txtMIN1 || wetness >= txtMAX2 || wetness <= txtMIN2)
                        server4.SendMessage("HighlightFlashingOpen");
                    else
                        server4.SendMessage("HighlightFlashingClose");
                    if (temperatureA >= txtMAX1 || temperatureA <= txtMIN1 || wetnessA >= txtMAX2 || wetnessA <= txtMIN2)
                        serverM.SendMessage("HighlightFlashingOpen");
                    else
                        serverM.SendMessage("HighlightFlashingClose");
                    if (temperatureB >= txtMAX1 || temperatureB <= txtMIN1 || wetnessB >= txtMAX2 || wetnessB <= txtMIN2)
                        serverL.SendMessage("HighlightFlashingOpen");
                    else
                        serverL.SendMessage("HighlightFlashingClose");
                }
                else 
                {
                    server4.SendMessage("HighlightFlashingClose");
                    serverM.SendMessage("HighlightFlashingClose");
                    serverL.SendMessage("HighlightFlashingClose");
                }
                if (temperature >= txtMAX1 || temperature <= txtMIN1) 
                {
                    Text_Temp0.color = Color.red;
                    Text_MinTemp.color = Color.red;
                    Text_MaxTemp.color = Color.red;
                    Text_AveTemp.color = Color.red;

                }
                else 
                { 
                    Text_Temp0.color = new Color32(0, 0, 0, 0);
                    Text_MinTemp.color = Color.black;
                    Text_MaxTemp.color = Color.black;
                    Text_AveTemp.color = Color.black;
                }

                if (temperatureA >= txtMAX1 || temperatureA <= txtMIN1)
                {
                    Text_Temp1.color = Color.red;
                    Text_MinTempA.color = Color.red;
                    Text_MaxTempA.color = Color.red;
                    Text_AveTempA.color = Color.red;
                }
                else 
                { 
                    Text_Temp1.color = new Color32(0, 0, 0, 0);
                    Text_MinTempA.color = Color.black;
                    Text_MaxTempA.color = Color.black;
                    Text_AveTempA.color = Color.black;
                }
                if (temperatureB >= txtMAX1 || temperatureB <= txtMIN1) 
                {
                    Text_Temp2.color = Color.red;
                    Text_MinTempB.color = Color.red;
                    Text_MaxTempB.color = Color.red;
                    Text_AveTempB.color = Color.red;
                }
                else 
                { 
                    Text_Temp2.color = new Color32(0, 0, 0, 0);
                    Text_MinTempB.color = Color.black;
                    Text_MaxTempB.color = Color.black;
                    Text_AveTempB.color = Color.black;
                }
                if (wetness >= txtMAX2 || wetness <= txtMIN2)
                {
                    Text_Wet0.color = Color.red;
                    Text_MinWet.color = Color.red;
                    Text_MaxWet.color = Color.red;
                    Text_AveWet.color = Color.red;
                }
                else 
                { 
                    Text_Wet0.color = new Color32(0, 0, 0, 0);
                    Text_MinWet.color = Color.black;
                    Text_MaxWet.color = Color.black;
                    Text_AveWet.color = Color.black;
                }
                if (wetnessA >= txtMAX2 || wetnessA <= txtMIN2)
                {
                    Text_Wet1.color = Color.red;
                    Text_MinWetA.color = Color.red;
                    Text_MaxWetA.color = Color.red;
                    Text_AveWetA.color = Color.red;
                }
                else 
                { 
                    Text_Wet1.color = new Color32(0, 0, 0, 0);
                    Text_MinWetA.color = Color.black;
                    Text_MaxWetA.color = Color.black;
                    Text_AveWetA.color = Color.black;
                }
                if (wetnessB >= txtMAX2 || wetnessB <= txtMIN2)
                {
                    Text_Wet2.color = Color.red;
                    Text_MinWetB.color = Color.red;
                    Text_MaxWetB.color = Color.red;
                    Text_AveWetB.color = Color.red;
                }
                else 
                { 
                    Text_Wet2.color = new Color32(0, 0, 0, 0);
                    Text_MinWetB.color = Color.black;
                    Text_MaxWetB.color = Color.black;
                    Text_AveWetB.color = Color.black;
                }
            }

            //AlarmCanvas的开关控制
            if (AlarFire == 1)
            {
                Text_AlarFire.text = "开关：开";
                if (fire == 0 && water == 0)
                { 
                    box1.SendMessage("HighlightFlashingClose");
                    //B1txtResult.color = Color.black;
                }
                if (fire == 0)//没报警
                {
                    txtResult.text += "无火警";
                    Text_Fire.text = "火警：无火警";  //多维度中显示
                    Text_Fire.color = Color.black;
                    Text_Fire1.text = "火警：无火警";
                    Text_Fire1.color = Color.black;
                    B1txtResult.text = "无火警";                    
                }
                else         //报警
                {
                    txtResult.text += "火警";
                    Text_Fire.text = "火警：火警";
                    Text_Fire.color = Color.red;
                    Text_Fire1.color = Color.red;
                    Text_Fire1.text = "火警：火警";
                    Text_Fire1.color = Color.red;                   
                    B1txtResult.text = "火警";
                    box1.SendMessage("HighlightFlashingOpen");
                    //B1txtResult.color = Color.red;
                }
            }
            else
            {
                Text_AlarFire.text = "开关：关";             
            }
            if (AlarWater == 1)
            {
                Text_AlarWater.text = "开关：开";
                if (fire == 0 && water == 0)
                {
                    box1.SendMessage("HighlightFlashingClose");
                   // B1txtResult.color = Color.black;
                }

                if (water == 0)
                {
                    txtResult.text += "无水浸";
                    Text_Water.text = "水浸：无水浸";
                    Text_Water.color = Color.black;
                    Text_Water1.text = "水浸：无水浸";
                    Text_Water1.color = Color.black;
                    B1txtResult.text += "\n无水浸";
                   // B1txtResult.color = Color.black;
                }
                else
                {
                    txtResult.text += "水浸";
                    Text_Water.text = "水浸：水浸";
                    Text_Water.color = Color.red;
                    Text_Water1.text = "水浸：水浸";
                    Text_Water1.color = Color.red;
                    B1txtResult.text += "\n水浸";
                   // B1txtResult.color = Color.red;
                    box1.SendMessage("HighlightFlashingOpen");
                }
            }
            else
            { 
                Text_AlarWater.text = "开关：关";             
            }
            if (AlarWindow == 1)
            {
                Text_AlarWindow.text = "开关：开";
                if (window1 == 0)
                {
                    txtResult.text += "门窗一关好";
                    Text_Window.text = "门窗：门窗一关好";
                    Text_Window.color = Color.black;
                    Text_Window1.text = "门窗：门窗一关好";
                    Text_Window1.color = Color.black;
                    PtxtResult.text = "门窗一关好";
                    PtxtResult.color = Color.black;
                    panel.SendMessage("HighlightFlashingClose");
                }
                else
                {
                    txtResult.text += "门窗一未关好";
                    Text_Window.text = "门窗：门窗一未关好";
                    Text_Window.color = Color.red;
                    Text_Window1.text = "门窗：门窗一未关好";
                    Text_Window1.color = Color.red;
                    PtxtResult.text = "门窗一未关好";                    
                    PtxtResult.color = Color.red;
                    panel.SendMessage("HighlightFlashingOpen");
                }
                if (window2 == 0)
                {
                    txtResult.text += "门窗二关好";
                    Text_Window0.text = "门窗：门窗二关好";
                    Text_Window0.color = Color.black;
                    Text_Window11.text = "门窗：门窗二关好";
                    Text_Window11.color = Color.black;
                    P1txtResult.text = "门窗二关好";
                    P1txtResult.color = Color.black;
                    panel1.SendMessage("HighlightFlashingClose");
                }
                else
                {
                    txtResult.text += "门窗二未关好";
                    Text_Window0.text = "门窗：门窗二未关好";
                    Text_Window0.color = Color.red;
                    Text_Window11.text = "门窗：门窗二未关好";
                    Text_Window11.color = Color.red;
                    P1txtResult.text = "门窗二未关好";
                    P1txtResult.color = Color.red;
                    panel1.SendMessage("HighlightFlashingOpen");
                }
            }
            else
            {
                Text_AlarWindow.text = "开关：关";               
            }
          isChanged = false;
        }
    }
    #endregion

    #region 下边四个按钮点击事件
    public void BtnManage()
    {        
    }
    public void BtnAlarm()
    {
        //只有自己显示
        if (AlarmCanvas.transform.GetComponent<CanvasGroup>().alpha == 1)
        {
            AlarmCanvas.SendMessage("Hide");
        }
        else
            AlarmCanvas.SendMessage("Show");
        InquireCanvas.SendMessage("Hide");
        AnalyseCanvas.SendMessage("Hide");
    }
    
    public void BtnInquire()
    {
        if (InquireCanvas.transform.GetComponent<CanvasGroup>().alpha == 1)
        {
            InquireCanvas.SendMessage("Hide");
        }
        else
            InquireCanvas.SendMessage("Show");
        AlarmCanvas.SendMessage("Hide");
        AnalyseCanvas.SendMessage("Hide");
    }
    public void BtnAnalyse()
    {
        if (AnalyseCanvas.transform.GetComponent<CanvasGroup>().alpha == 1)
        {
            AnalyseCanvas.SendMessage("Hide");
        }
        else
            AnalyseCanvas.SendMessage("Show");
        AlarmCanvas.SendMessage("Hide");
        InquireCanvas.SendMessage("Hide");
    }
    #endregion
    //——————————————————————————————————————————————————————————————
//箱子1 M L
    public void Server4()
    {
        server4Canvas.SendMessage("Show");

    }
    public void BtnS4Close() 
    {
        server4Canvas.SendMessage("Hide");
    }
    public void sever_M()
    {
        serverMCanvas.SendMessage("Show");

    }
    public void BtnMClose()
    {
        serverMCanvas.SendMessage("Hide");
    }
    public void sever_L()
    {
        serverLCanvas.SendMessage("Show");

    }
    public void BtnLClose()
    {
        serverLCanvas.SendMessage("Hide");
    }
    //门窗1 
    public void Panel()
    {
        PanelCanvas.SendMessage("Show");

    }
    public void Panel1()
    {
        PanelCanvas1.SendMessage("Show");

    }
    public void BtnPClose()
    {
        PanelCanvas.SendMessage("Hide");
    }
    public void BtnP1Close()
    {
        PanelCanvas1.SendMessage("Hide");
    }

    //火警水浸
    public void Boxl()
    {
        BoxCanvas1.SendMessage("Show");

    }
    public void BtnB1Close()
    {
        BoxCanvas1.SendMessage("Hide");

    }
    //AlarmCanvas
    //
    public void ButAlarTemp()
    {

        if (AlarTemp == 0)
        {
            AlarTemp = 1;
            Debug.Log("AlarTemp=" + AlarTemp);
        }
        else
            AlarTemp = 0;
    }
    public void ButAlarFire()
    {
        if (AlarFire == 0) {
            AlarFire = 1;
            Debug.Log("AlarFire=" + AlarFire);
        }
        else
            AlarFire = 0;
    }
    public void ButAlarWater()
    {
        if (AlarWater == 0)
        {
            AlarWater = 1;
        }
        else
            AlarWater = 0;
    }
    public void ButAlarWindow()
    {
        if (AlarWindow == 0)
        {
            AlarWindow = 1;
        }
        else
            AlarWindow = 0;
    }

}

//自定义数据比较器，对于 Compare 方法，两个值相返回 0,当 x>y 返回值大于 0,当 x<y 返回值小于 0
class MyCompare : IComparer
{
    public int Compare(object x, object y)
    {
        double str1 = Convert.ToDouble(x);
        double str2 = Convert.ToDouble(y);
        return str1.CompareTo(str2);//返回从小到大
    } 
}