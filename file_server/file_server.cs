using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
            var transport = new Transport(BUFSIZE, APP);
		    var receiveBuffer = new byte[BUFSIZE];
		    var receivedData = new byte[]{};

		    while (true)
		    {
		        try
		        {
		            var receiveSize = 0;
		            int index = 0;

                    Console.WriteLine("Awaiting new file request");

		            do
		            {
		                receiveSize = transport.receive(ref receiveBuffer);
		                Array.Resize(ref receivedData, receiveSize);
		                Array.Copy(receiveBuffer, 0, receivedData, index, receiveSize);
		                index += receiveSize;

		            } while (receiveSize == BUFSIZE);

		            string receivedFilePath = Encoding.UTF8.GetString(receivedData);

		            var requestedFile = LIB.extractFileName(Encoding.UTF8.GetString(receivedData));
		            Console.WriteLine("Extracted " + requestedFile + "from client.");

		            //til filer på vilkårlige placeringer
		            var fileLength = LIB.check_File_Exists(receivedFilePath);

		            if (fileLength > 0) //tjekker om filen findes på den givne sti
		            {
		                Console.WriteLine($"Fuld sti:{receivedFilePath}" +
		                                  $"\nstørrelse:{fileLength}");
		                sendFile(receivedFilePath, fileLength, transport);
		            }
		            else
		            {
                        Console.WriteLine($"Fuld sti:{receivedFilePath}" +
                                          $"\nFil ikke fundet.");
		                var zeroArray = new byte[] { 0 };
		                transport.send(zeroArray, zeroArray.Length);
		            }
                }
		        catch (Exception e)
		        {
		            Debug.WriteLine(e);
		            throw;
		        }
		    }
        }

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
		    byte[] sizeArray = Encoding.UTF8.GetBytes(fileSize.ToString());
		    transport.send(sizeArray, sizeArray.Length);

		    var fileByteList = File.ReadAllBytes(fileName).ToList();
		    var splitFileByteList = splitList(fileByteList, BUFSIZE);
		    foreach (List<byte> bytes in splitFileByteList)
		    {
		        transport.send(bytes.ToArray(), bytes.Count);
		    }
		    if (splitFileByteList[splitFileByteList.Count - 1].Count == BUFSIZE) //if last list == buffsize, send empty message to signify end of transmissions
		    {
		        transport.send(new byte[0], 0);
		    }
            Console.WriteLine($"Fil sendt");
		}
         
	    private static List<List<byte>> splitList(List<byte> byteList, int nSize = 1000)
	    {
	        var list = new List<List<byte>>();
	        for (int i = 0; i < byteList.Count; i += nSize)
	        {
	            list.Add(byteList.GetRange(i, Math.Min(nSize, byteList.Count - i)));
	        }
	        return list;
	    }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main (string[] args)
		{
			new file_server();
        }
	}
}