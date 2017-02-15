using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Signin_Grabber {
	class Program {
		static void Main(string[] args) {

			#region initialization
			// Filters for what to keep
			bool keepHorizontal = false;
			bool keepVertical = false;

			string filter = ConfigurationManager.AppSettings["filter"];
			if (filter == "Both") {
				keepHorizontal = true;
				keepVertical = true;
			} else if (filter == "Horizontal") {
				keepHorizontal = true;
			} else if (filter == "Vertical") {
				keepVertical = true;
			} else {
				// if it is neither of the values, we write out the error and close the program
				Console.WriteLine("Filter has invalid value, only Horizontal, Vertical and Both are permitted");
				Environment.Exit(0);
			}

			//paths we need for knowing where to copy from and where to
			string destination = ConfigurationManager.AppSettings["destination"];
			if (Directory.Exists(destination) == false) {
				Console.WriteLine("Destination directory " + destination + " does not exist");
				Environment.Exit(0);
			}

			string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

			string source = home + "\\" + ConfigurationManager.AppSettings["position"];
			if (Directory.Exists(source) == false) {
				Console.WriteLine("Source directory " + source + " does not exist");
				Environment.Exit(0);
			}

			// minimum size of the file
			int minimumSize = int.Parse(ConfigurationManager.AppSettings["minimumSize"]);
			#endregion

			// population af hashtable - bruger en hashtable, fordi det er en pokkers 
			// nem måde at få at vide om nøglen allerede findes :p
			Hashtable h = new Hashtable();

			/* For hver fil i destinationsmappen fjerner vi extension, og tilføjer den 
			 * til hash tabellen. På den måde vil vi nemt kunne lave et opslag, og i 
			 * source folderen har filerne nemlig ikke en extension 
			 * */
			DirectoryInfo di = new DirectoryInfo(destination);
			FileInfo[] fi = di.GetFiles();
			foreach (FileInfo file in fi) {
				string name = Path.GetFileNameWithoutExtension(file.Name);
				h.Add(name, name);
			}

			/* Vi ændrer pegepinden, og går nu igennem source. Vi checker for tre ting
			 * 1) Størrelse
			 * 2) Alignment
			 * 3) Om den findes allerede
			 * */
			di = new DirectoryInfo(source);
			fi = di.GetFiles();
			foreach(FileInfo file in fi) {
				// Checker størrelsen, og hvis false skriver vi en informativ fejlbesked nedenfor
				if (file.Length >= minimumSize) {

					// Vi gør lidt forarbejde for at få adgang til informationer om billedet
					Image img = Image.FromFile(file.FullName);
					bool pictureIsHorizontal = true;
					if (img.Height > img.Width) {
						pictureIsHorizontal = false;
					}

					// Checker om billeden er horisontalt og vi ønsker at beholde det, eller det samme og vertikalt
					if (pictureIsHorizontal && keepHorizontal || pictureIsHorizontal == false && keepVertical) {

						// Vi checker om billedet findes, og hvis ikke, så kopierer vi det
						if (h.ContainsKey(file.Name) == false) {
							file.CopyTo(destination + file.Name + ".jpg");
							Console.WriteLine(file.Name + " copied to " + destination);

						} else {
							Console.WriteLine(file.Name + " already exists");
						}
					} else {
						Console.WriteLine(file.Name + " was discarded because it was " + (pictureIsHorizontal ? "horizontal" : "vertical"));
					}
				} else {
					Console.WriteLine(file.Name + " was discarded because length " + file.Length + " is less than limit " + minimumSize);
				}
			}

			Console.WriteLine("");
			Console.WriteLine("By courtesy of Kim Bahir Andersen, all rights reserved");
			Console.WriteLine("Version 1.0, Feel free to donate: " + "http://bit.ly/1XPj1VV");
			Console.WriteLine("");
			Console.WriteLine("Type enter to exit");
			Console.ReadLine();
		}
	}
}
