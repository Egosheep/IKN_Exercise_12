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
            /*#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttySn0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
				}
			#else
				
			#endif*/
		    serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
            if (!serialPort.IsOpen)
				serialPort.Open();

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
		    var bufferlist = new List<byte>();
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
            buffer = new byte[bufferlist.Count];
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
            var listBuffer = new List<Byte>();
		    var bufferlist = new List<byte>();
		    byte readByte;
		    do
		    {
		        readByte = (byte)serialPort.ReadByte();
		    } while (readByte != DELIMITER);

		    readByte = (byte)serialPort.ReadByte();
		    do
		    {
		        listBuffer.Add(readByte);
		        readByte = (byte)serialPort.ReadByte();
		    } while (readByte != DELIMITER);

		    for (int i = 0; i < listBuffer.Count; i++)
		    {
		        if (listBuffer[i] == DELIMITER)
		        {
		            
		        }
		        else if (listBuffer[i] == BBYTE)
		        {
		            if (listBuffer[i+1] == CBYTE)
		            {
		                bufferlist.Add(ABYTE);
		                i++;
		            }
                    else if (listBuffer[i+1] == DBYTE)
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
		    bufferlist.ToArray().CopyTo(buf, 0);
		    bufferlist.Clear();
            return bufferlist.Count;
        }
	}
}
