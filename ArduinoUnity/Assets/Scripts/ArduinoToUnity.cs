using System;
using System.IO.Ports;
using UnityEngine;


class ArduinoToUnity : MonoBehaviour
{
	SerialPort sp = new SerialPort ("COM6", 9600);
	bool isStreaming = false;

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
			string value = ReadSerialPort(100);
			Debug.Log (value);
		}
	}

}
