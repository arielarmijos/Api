README:

	Dado que el API incluye referencias a librerias de Oracle de x64 se presentan problemas especiales
al momento de correr la aplicacion en una maquina de desarrollo con Windows 7. Hasta el momento la unica
forma en la que podemos correr esta aplicacion localmente es utilizando un servidor IIS Local y correr 
la aplicación alli. Deben tomarse en cuenta las siguientes previsiones:

	1. Visual Studio debe estar corriendo con una cuenta de Administrator.
	2. Para no generar cambios en el proyecto, debe existir una aplicación en el IIS llamada api-movilway-dev.

	Esta documentado que la version 8.0 de IIS Express va a soportar aplicaciones de 64bits de forma nativa
lo que implica que posiblemente se pueda utilizar, se realizaron pruebas con el Release Candidate de la 8.0
y se vio que la integración con Visual Studio no es la apropiada y se intentan correr las apps a 32bits.