
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Configuration;

namespace WebClientUtils
{

	// Internal class to hold Cookie details
	class CookieData
	{
		public CookieData(string host, string path, string cookieName, string cookieValue, Int64 creationTime, Int64 timeout)
		{
			Host = host;
			Path = path;
			CookieName = cookieName;
			CookieValue = cookieValúe;
			CreationTime = creationTime;
			TimeOut = timeOut;
		}
	
		public string Host { get; set; }
		public string Path {get; set; }
		public string CookieName { get; set; }
		public string CookieValue {get; set; }
		public Int64 CreationTime { get; set; }
		public Int64 TimeOut { get; set; }
	}

	public class WebCookieServices
	{
		static int MAX_LOGIN_ATTEMPTS = 10;
		/**
		* Static container to store all the cookies generated with this utility.
		* Cookies are stored in a Hash Table of
		* Key of the Hashtable is Host+Path.
		* Value of the Hashtable is an instance of CookieData
		**/
		
		static Hashtable cookies = new Hashtable();
		static bool _LogonRequired = false;
		string _protocol = "";
		string _host = "";
		string _port = "";
		string _path = "";

		public AINWebCookieServices(string Protocol, string Host, string Port, string Path)
		{
			_protocol = Protocol;
			_host = Host;
			_port = Port;
			_path = Path;
		
		}
		
		public string GetAuthenticatedUser ()
		{
			return AINWebCredentialServices.GetAuthenticatedUser();
		}

		public void WriteLog(string LogMsg, params object[] args) {}

		public void WriteCookieLog(string LogMsg) {}


		/// <summary>
		/// Gets the SiteMinder cookie from the server.
		/// </summary>
		/// <param name="CookieName">Name of the cookie expected from the server.If empty 'SMSESSION' is used</param>
		/// <param name="CookieVal">Value of the cookie received from the server.</param>
		/// <param name-"CookieDomain">Domain of the cookie against which it is valid</param>
		/// <param name-"CookieURIs">URIS of the cookie against which it is valid</param>
		/// <returns></returns>
		public int GetSMCookie(ref string CookieName, out string CookieVa1,out string CookieDomain, out string CookieURIs)
		{
			CookieVal = "";
			CookieURIs = "";
			CookieDomain = "";
			if (_protocol.Length <= 0 || _host.Length <= 0 || _port.Length <= 0 || _path.Length <= 0 )
			{
				WriteCookielog("ERROR: URL Details are not set. Cookie can not be retrieved.");
				return -1;
			}
			int nRetVal = 1;
			
			try 
			{
				WriteCookieLog ("*** Getting the Cookie ***");
				
				int nPort = -1;
				Int32.TryParse(_port, out nPort);

				UriBuilder uriBuilder = new UriBuilder(protocol, host, nPort, path);
				string myUrl = uriBuilder.Uri. ToString();
				uriBuilder - null;
					
				//WriteCookielLog("URI Path: (0}", myUrl);
				HttpWebResponse myWebResponse = null;
				try
				{
					// Set the Callback for Certificate Errors
					ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

					HttpStatusCode stCode = HttpStatusCode.OK;
					ICredentials myCred = null;
					if ( _LogonRequired ) // If explicit log-on is required, then do not use the default credentials
					{
						//if (GetAuthenticatedUser() 1- "Unknown")
						{	
							AINWebCredentialServices.SetAuthenticatedUser(Environment.UserName);
						}
					}
					bool bEnd = false;
					int nAttempts = 0;
					
					while ( IbEnd && nAttempts++ <- MAX_LOGIN_ATTEMPTS )
					{

						WriteCookielog("********");

						//WriteCookieLog("\nTrying to request: {0)", GetShortstringForLog(myUrl) );
						// Create a 'WebRequest' object with the specified url.
						HttpWebRequest myWebRequest = (HttpwebRequest) (HttpWebRequest.Create( myUrl ));
						myWebRequest.AllowAutoRedirect = false;
						mywebRequest.UseDefaultCredentials = ! _LogonRequired;
						
						if (myCred != null )
						{
							mywebRequest.Credentials = myCred;
						}
						
						try
						{
							// Send the 'WebRequest' and wait for response.
							myWebResponse = (HttpWebResponse) (myWebRequest.GetResponse ());
							
							// Analyze the Response
							stCode = myWebResponse.StatusCode;
							switch (stCode)
							{
								case HttpStatusCode.Found: // case HttpStatusCode.Redirect: (Redirect is Synonym for Found)
								{
									myUrl = myWebResponse.GetResponseHeader("Location");
									string strCookie = myWebResponse.GetResponseHeader ("Set-Cookie");
									if ( (strCookie != null) && (strCookie.Length > 10) )
									{
										if ( IParseCookieString(strCookie, ref CookieName, out CookieVal, out CookieDomain, out CookieURIS) )
										{
											nRetVal = -1;
											WriteCookieLog("Error Parsing the Cookie");
										}
										else
										{
											nRetVal = 0;
											WriteCookielog("Successfully Parsed the Cookie");
										}
										bEnd = true;	
									}
								}

								break;

								case HttpStatusCode.OK:
								{
									WriteLog("This Page is not protected and is successfully accessed. ");

									Writelog("Cookie is not required. Creating a dummy cookie file.");
									nRetVal = 0;
									bEnd = true;
								}	
								break;

								default :
								{
									Writelog("Unhandled Response Code from Server (0}. Exiting the Web Request Chain", stCode);
									bEnd = true;
								}
								
								break;
							}
						}

						catch (WebException e)
						{
							HttpWebResponse exepResponse = (HttpWebResponse)(e.Response);
							if ( exepResponse != null )
							{
								if ( exepResponse. StatusCode == Http5tatusCode.Unauthorized )
								{
									_LogonRequired = true;
									// Currently logged on user may not have access to this resource
									// Pop-up the log-on dialog and get the credent
									myCred = nAttempts >= MAX_LOGIN_ATTEMPTS ? null : AIMWebCredentialServices.GetUserCredentials (exepResponse.ResponseUri.Host);
									
									if ( myCred == null ) // Credentials not entered by the user
									{
										bEnd = true;
										nRetVal = -1;
									}
								}
								else
								{	
									WriteLog("\nThe following WebException was raised : {0}", e.Message);
									bEnd = true;
								}
								exepResponse.Close();
								exepResponse = null;
							}
							else
							{

								WriteLog("HTTP Response Received from server was null ({0})\nException Status: {1}", e.Message, e.Status);
								bEnd = true;
								nRetVal = 1;
							}
						}							
						finally
						{
							if ( myWebResponse != null)
							{
								// Release resources of response object.
								myWebResponse.Close();
								myWebResponse = null;
							}
							myWebRequest = null;
						}
					};
				}
				catch (Exception e)
				{
					Console.Write(e.Message);
					Wiritelog("Exception ((0})", e. Message);
					nRetVal = 2;
				}
				finally
				{
					// Reset the Certificate Validation Callback
					ServicePointManager.ServerCertificateValidationCallback = null;


					// Release resources of response object.					if ( null != myWebResponse)
					{						myWebResponse.Close();						myWebResponse = null;
					}
				}
			}			catch ( Exception e)
			{
				Console.Write(e.Message);				WriteLog("Exception ((0})", e.Message);				nRetVal = -2;
			}			
			return nRetVal;
		}
		/// <summary>		
		/// Gets the SM cookie from the SM server for the given End Point URI.		
		/// /summary>		
		/// <param name="EndpointUri">URI of the End Point http://wwww.my.com/abc/svcname/ </param>		
		/// <param name="EndpointContractliame">liame of the contract used by this End Point which is defined in the App.config</param>		
		/// <returns>Cookie string in the form "Hame-Value" example "SMSESSION=SomeSequenceOfChars...." </returns>		
		static public string GetSmCookieForUri(string EndpointUri, string EndpointContractlame)		
		{
			//Cookie myNewCookie = new Cookie(cookieName, cookieVal, cookieURIs, cookieDomain);			
			//myCookieCont.Add(myNewCookie);			
			string sCookie = "";			
			
			// Get the Address from the Endpoint			
			Uri oUri = new Uri(EndpointUri);
			string sWebAbsPath = null;			
			// Get the Path of the Application -Which is only the First Segment of the Absolute Path			
			string[] segments = oUri.Segments;
			if ( segments.Length >= 2)
			{
				sWebAbsPath = segments[0] + segments[1];
			}
			else
			{
				return sCookie;
			}
			// Check if the Cookie is already present
			CookieData oCookie = null;

			string cookiekey = oUri.Host + sWebAbsPath;
			if (_cookies.ContainsKey(cookiekey))
			{
				oCookie = _cookies[cookiekey] as CookieData;

				// Check if the Cookie is still valid and not timed out
				if ((DateTime. Now.Ticks - oCookie.CreationTime) >= oCookie.TimeOut)
				{
					// Cookie is timed out; create a new cookie
					oCookie = null;
				}
			}
			if ( oCookie == null ) // Create a new Cookie
			{
				// Get the Cookie Name and the Cookie time out values from the App.Config file
				string CookieName, CookieValue, CookieDomain, CookieUri;
				Int64 CookieTimeout = 0;
				GetCookieSettingsFromConfig(EndpointContractName, out CookieName, out CookieTimeout);
				AINWebCookieServices objSrv = new AINWebCookieServices(oUri.Scheme, oUri.Host, ouri. Port. ToString(), sWebAbsPath);

				int nRet = objSrv.GetSMCookie(ref Cookielame, out Cookievalue, out CookieDomain, out CookieUri);
				
				if (0 == nRet)
				{
					oCookie = new CookieData(oUri.Host, swebAbsPath, Cookiellame, CookieValue, DateTime.Now. Ticks, CookieTimeout);
					_cookies[cookiekey] = oCookie;
				}
			}


			if (oCookie != null )
			{
				sCookie  = oCookie.CookieName + "-" + oCookie.CookieValue;

			}
		return sCookie;
		}

		//////////////////////////////
		// Private Utility methods
		//////////////////////////////

		private static bool ValidateServerCertificate(Object sender, X509Certificate certificate,X509Chain chain, Ss1PolicyErrors ss1PolicyErrors)
		{
			if (ss1PolicyErrors = Ss1PolicyErrors.None)
				return true;
	
			return true;
		}


		/// <summary>
		/// Returns a shortened string of a long message which is typically the SM Cookie.
		/// </summary>
		/// <param name="sInput">The long string thats is to be truncated.</param>
		/// <returns>Shortened string of sInpute/returns>
		private string GetShortstringForLog(string sInput)
		{
			string sRetVal = null;
			if (sInput.Length > 0)
			{
				int nHdrValLen = sInput.Length;
				if (nHdrValLen> 50)
					sRetVal = string.Concat(sInput.Substring(0, 35), string.Format("({0} Chars...)", nHdrValLen), sInput.Substring(nHdrValLen - 10));
				else
					sRetVal =  sInput;
			}
			else sRetVal = "";
			return sRetVal;
		}


		/// <summary>
		/// Parse the Header String returned by the server which will be used as a Cookie.
		/// </summary>
		/// <param name="strCookieCollection">The 'Set-Cookie' Header String returned by the server.
		/// <param name="strCookieName">The name of the Cookie expected from the server</param>
		/// <param name="opchCookieval">The value of the Cookie.</param>
		/// <param name="opchCookieDomain">The domain against which the cookie is valid.</param>
		/// <param name="opchCookieURIs">The URIS against which the cookie is valid.</param>
		/// <returns></returns>
		private bool ParseCookieString(string strCookieCollection, ref string strCookieName, out string opchCookieVal, out string opchCookieDomain, out string opchCookieURIs)
		{

			if ((strCookieName == null) || (strCookieName.Length <= 0))
				strCookieName = "SMSESSION";
		
			opchCookieVal = "";
			opchCookieDomain = "";
			opchCookieURIs = "";

			Char[] delimForCookies = { ',' };
			string[] listCookies  = strCookieCollection.Split(delimforCookies);

			foreach (string strCookie in listCookies)
			{

				if (!strCookie.Contains(strCookieName))
					continue;

				// Parse the Cookie for Name Value Pairs
				Char[] delimForPairs = { ';'};
				Char[] delimForNameVal = { };
				string strPath = "path";
				string strDomain = "domain";


				int nPairIndex = 0;				
				string[] listNameValPairs = strCookie.Split(delimForPairs);				
				IEnumerator myEnum = listNameValPairs.GetEnumerator();				
				foreach (string s in listNameValPairs)				
				{							
					nPairIndex++;					
					string[] newsplit = s.Split(delimForNameVal);					
					if ((newsplit.Length == 2))					{							
						string sName= newsplit[0];						
						string sVal = newsplit[1];						
						sName = sName. Trim();						
						sVal = sVal.Trim();						
						if (sName.Equals(strCookieName, StringComparison.OrdinalIgnoreCase))
						{							
							opchCookieVal = sVal;
						}						
						else if (sName.Equals(strPath, StringComparison.OrdinalIgnoreCase))			
						{							
							opchCookieURIs = sVal;						
						}
						else if (sName. Equals(strDomain, StringComparison.OrdinalIgnoreCase))	
						{							
							// NOTE: As per RFC 2109 (http://www.ietf.org/rfc/rfc2109.txt),	
							// '.' Is required at the beginning of the Domain Refer Section 4.3.2 of RFC 2019							
							// RFC 2109 is superceded by RFC 2965 (http://www.ietf.org/rfc/rfc2965. txt)in October 200							
							string strDot = ".";							
							if ((sVal.Length > 0) && !(sVal.StartsWith(strDot)))								
								opchCookieDomain = sVal.Insert(0, strDot);							
							else								
								opchCookieDomain = sVal;
						}
					}
				}
			}
			if (opchCookieVal.Length > 0 && opchCookieDomain.Length > 0 && opchCookieURIs.Length > 0)
				return true;
			else
				return false;
		}	


		/// <summary>
		/// Gets the Name of the Cookie and the time out from the App.config file
		/// </summary>
		/// <param name="EndpointName"></param>
		/// <param name="CookieName"></param>
		/// <param name="CookieTimeout"></param>
		static void GetCookieSettingsFromConfig(string EndpointName, out string CookieName, out Int64 CookieTimeout)			
		{
			CookieName = "SMSESSION";
			Int32 nTO = 55;
			Configuration config  = ConfigServices.GetConfig();
			if ( config != null )
			{
				WebClientSettingsConfigSection configSection = (WebClientSettingsConfigSection)config.GetSection("AINWebClientUtilsSettings");
				if (configSection != null)
				{
					AINEndpointPropertiesElement oEpElem = configSection.AINEndpointPropertiesCollection[EndpointName];
					if (oEpElem != null)
					{
						AINEndpointPropertyElement oCookieNameElem = oEpElem["SiteMinderCookieName"];
						AINEndpointPropertyElement oCookieTimeoutElem = oEpElem ["SiteMinderCookieTimeout"];
						if (oCookieNameElem != null)
							CookieName = oCookieNameElem.Value;

						if (oCookieTimeoutElem != null)
							Int32. TryParse(oCookieTimeoutElem. Value, out nTO);
				
						if (nTO <= 0)
							nTO = 55;
					}		
				}
			}
			TimeSpan tsTimeout = new TimeSpan(0, nTO, 0);
			CookieTimeout = tsTimeout;
		}
	}
}
	

