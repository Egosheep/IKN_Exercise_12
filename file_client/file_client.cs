using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
	        Transport transport = new Transport(BUFSIZE, APP);
	        receiveFile(args[0], transport);
        }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{
		    var receiveBuffer = new byte[1000];
		    var receivedData = new byte[] { };
            var fileNameArray = Encoding.UTF8.GetBytes(fileName);
            transport.send(fileNameArray, fileNameArray.Length);
		    var filePath = AppDomain.CurrentDomain.BaseDirectory + "/" + fileName;

            var existCheck = new byte[]{};
		    transport.receive(ref existCheck);
		    if (existCheck[0] != 0)
		    {
		        var receiveSize = transport.receive(ref receiveBuffer);
		        int index = 0;
		        do
		        {
		            Array.Copy(receiveBuffer, 0, receivedData, index, BUFSIZE);
		            receiveSize = transport.receive(ref receiveBuffer);
		            index += 1000;
		        } while (receiveSize == BUFSIZE);
		        if (receiveSize > 0)
		        {
		            Array.Copy(receiveBuffer, 0, receivedData, index, receiveBuffer.Length);
		        }
		        if (File.Exists(filePath))
		        {
		            File.Delete(filePath);
		        }
		        File.WriteAllBytes(filePath, receivedData);
		    }
		}

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// First argument: Filname
        /// </param>
        public static void Main (string[] args)
		{
			new file_client(args);
		}
	}
}