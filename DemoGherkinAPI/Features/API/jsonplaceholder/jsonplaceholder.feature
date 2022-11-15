Feature: jsonplaceholder.typicode.com

As a jsonplaceholder.typicode.com user, i want to call  endpoints to show how 
Gherkin works


@GETExamples @API @jsonplaceholder
Scenario: Todos
	Given the rest service available with the <endpoint> endpoint
	And I use the security configuration from Resources/jsonplaceholder/AuthorizationNone.config file
	And a collection of path params <pathParam> content 
	When I perform the GET action 
	And I send the request
	Then the HTTP response should be 200 
	And the result should match the expected Resources/jsonplaceholder/Response/<jsonResponse> response
Examples:
 | title | endpoint | pathParam | jsonResponse |
 | 1     | todos    | 1         | id1.json     |
 | 5     | todos    | 5         | id5.json     |
 | 100   | todos    | 100       | id100.json   |
 | all   | todos    |           | idAll.json   |


 @PostExamples @API @jsonplaceholder
Scenario: Post
	Given the rest service available with the <endpoint> endpoint
	And I use the security configuration from Resources/jsonplaceholder/AuthorizationNone.config file
	When I perform the POST action 
	And I send the Resources/jsonplaceholder/Request/<jsonResquest> json request content
	Then the HTTP response should be 201 
	And the result should match the expected Resources/jsonplaceholder/Response/<jsonResquest> response
Examples:
 | title       | endpoint | jsonResquest    |
 | New element | posts    | NewElement.json |
 
 



