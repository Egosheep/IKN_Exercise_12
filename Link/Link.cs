using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = ABYTE;
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

	    private const byte ABYTE = (byte) 'A';
	    private const byte BBYTE = (byte)'B';
	    private const byte CBYTE = (byte)'C';
	    private const byte DBYTE = (byte)'D';

        /// <summary>
        /// Initializes a new instance of the <see cref="link"/> class.
        /// </summary>
        public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttySn0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
		    var bufferlist = new List<byte> { Capacity = 2008 };
		    bufferlist.Add(DELIMITER);
		    for (int i = 0; i < buf.Length; ++i)
		    {
		        if (buf[i] == DELIMITER)
		        {
		            bufferlist.Add(BBYTE);
		            bufferlist.Add(CBYTE);
		        }
		        else if (buf[i] == BBYTE)
		        {
		            bufferlist.Add(BBYTE);
		            bufferlist.Add(DBYTE);
		        }
		        else
		        {
		            bufferlist.Add(buf[i]);
		        }
		    }
		    bufferlist.Add(DELIMITER);

		    buffer = bufferlist.ToArray();
        
            serialPort.Write(buffer, 0, buffer.Length);
		    buffer.ToList().Clear();
        }

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
            serialPort.Read(buffer, 0, 2008);
	    	var bufferlist = new List<byte>();
		    for (int i = 0; i < buffer.Length; i++)
		    {
		        if (buffer[i] == DELIMITER)
		        {
		            
		        }
		        else if (buffer[i] == BBYTE)
		        {
		            if (buffer[i+1] == CBYTE)
		            {
		                bufferlist.Add(ABYTE);
		                i++;
		            }
                    else if (buffer[i+1] == DBYTE)
		            {
		                bufferlist.Add(BBYTE);
		                i++;
		            }
		        }
		        else
		        {
		            bufferlist.Add(buffer[i]);
		        }
		    }
		    buf = bufferlist.ToArray();
		    bufferlist.Clear();
            return bufferlist.Count;
        }
	}
}
