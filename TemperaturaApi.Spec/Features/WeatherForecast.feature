Feature: Weather Forecast Web Api
	Web API for weather forecasts

Scenario: Get weather forecasts
	Given I am a client
	And the repository has weather data
	When I make a GET request to 'weatherforecast'
	Then the response status code is '200'
	And the response json should be 'weathers.json'