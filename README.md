# ClientAuth
Client side application to authenticate http SOAP based request with C# .Net
Example shows two authentication scheme used:
1) Siteminder SSO Scheme
2) SAML SSO Scheme

As a test I have used an online SOAP URL endpoint:
http://www.dneonline.com/calculator.asmx?wsdl

Note: This URL is not protected via any scheme, but in actual environment we can replace it with actual protected URL.

Solution consists of 3 projects:
1) WsdlClient : Wraps WSDL endpoint to a C# interface
2) WebClientUtils: Utility classes that intercepts http requests via .Net ServiceModel extensions
3) CalculatorServices: Test client project to make the http requests to the WSDL endpoint.
