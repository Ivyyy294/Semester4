using System;
using System.IO.Ports;
using UnityEngine;


class UnityToArduino : MonoBehaviour
{
	[SerializeField] float threshold = 20f;
	SerialPort sp = new SerialPort ("COM6", 9600);
	bool ledOn = false;
	bool isStreaming;

	void SwitchLEDState(bool val)
	{
		ledOn = val;
		sp.WriteLine("L" + (ledOn ? "1" : "0"));
	}

	void OpenConnection()
	{
		isStreaming = true;
		sp.ReadTimeout = 100;
		sp.Open();
	}

	void Close()
	{
		sp.Close();
	}

	private void Start()
	{
		OpenConnection();
	}

	private void OnDisable()
	{
		Close();
	}

	private void Update()
	{
		if (isStreaming)
		{
			string strData = ReadSerialPort();
			Debug.Log (strData);
			
			try
			{
				float distance = float.Parse (strData);
				SwitchLEDState (distance <= threshold);
			}
			catch(Exception e)
			{
				ledOn = false;
			}

			
			//if (Input.GetKeyDown (KeyCode.Space))
			//	SwitchLEDState();
		}
	}

	string ReadSerialPort (int timeout = 50)
	{
		string message;

		sp.ReadTimeout = timeout;

		try
		{
			message = sp.ReadLine();
			return message;
		}
		catch (TimeoutException)
		{
			return null;
		}

	}

}
