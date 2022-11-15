Feature: herokuapp

AS a herokuapp user,  i want to login into the web using literal, variables and
english and spanish languages 

@herokuapp @UI
Scenario: Login HardCore
	Given I open the browser with the herokuapp home main page
	And the herokuapp home page is loaded successfully
	When I press the Link Form Authentication of the herokuapp home page
	And the herokuapp login page is loaded successfully
	And I login into the application herokuapp
	And the herokuapp secure page is loaded successfully
	And I press the Button logout of the herokuapp secure page
	Then the herokuapp login page is loaded successfully

@herokuapp @UI
Scenario Outline: Login with variables
	Given I open the browser with the <homePage> main page
	And the <homePage> page is loaded successfully
	When I press the Link <linkOption> of the <homePage> page
	And the <loginPage> page is loaded successfully
	And I login into the application <application>
	And the <securePage> page is loaded successfully
	And I press the Button <buttonOption> of the <securePage> page
	Then the <loginPage> page is loaded successfully
Examples:
 | title | homePage       | loginPage       | securePage       | linkOption          | buttonOption | application |
 | OK    | herokuapp home | herokuapp login | herokuapp secure | Form Authentication | logout       | herokuapp   |
 | KO    | herokuapp home | herokuapp login | herokuapp secure | Form Authentication | Norrrr       | herokuapp   |
 | KO2   | herokuapp home | herokuapp login | herokuapp secure | Exit Intent         | Norrrr       | herokuapp   |
 

 @herokuapp @UI
 Scenario: Login En castellano
	Given yo abro el navegador con la herokuapp home pagina principal
	And la herokuapp home pagina es cargado correctamente
	When yo presiono el Enlace Form Authentication de la herokuapp home pagina
	And la herokuapp login pagina es cargado correctamente
	And yo me logueo en la aplicacion herokuapp
	And la herokuapp secure pagina es cargado correctamente
	And yo presiono el Boton logout de la herokuapp secure pagina
	Then la herokuapp login pagina es cargado correctamente

