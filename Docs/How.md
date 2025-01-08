# How

Hoe ga ik het systeem eigenlijk maken?

## Architectuur

### Van buiten

![C4 layer 1](./C4_Layer1.drawio.png)

De gebruikers van het systeem zijn studenten die hun kookpunten en uitgaven bij willen houden.

![C4 layer 2](./C4_Layer2.drawio.png)

Deze gebruiker kan twee interfaces gebruiken: een mobiele app en een scherm dat aan de muur hangt. Deze gebruiken alle
twee SolidJS als frontend framework. Dit is een modern framework dat erg veel lijkt op React, met het voordeel dat het eenvoudiger in elkaar zit.

### Lagen van het systeem

![C4 layer 3](./C4_Layer3.drawio.png)

De API-applicatie is onderverdeeld in drie lagen: de API zelf, de logicalaag en de datalaag.

De API-laag gebruikt asp.net om een REST-api te maken. Deze API werkt dus met verschillende resources waarop acties worden uitgevoerd zoals toevoegen, aanpassen of verwijderen. De vorm van deze API wordt gepubliceerd via OpenAPI met Swagger, waardoor de front-end precies weet hoe de opgehaalde data eruit ziet. Ook wordt er veel gebruik gemaakt van websockets voor real-time data, waarbij AsyncAPI deze rol vervult.

De logica is de kern van de
applicatie. Deze is dus niet afhankelijk van de code van de andere twee lagen, maar gebruikt interfaces als er toch contact nodig is, zoals bij de datalaag vaak het geval is.

De datalaag gebruikt Entity Framework Core om te communiceren met de database. Dit is de standaard ORM van C#, die er onder andere voor zorgt dat er minder risico is op sql-injecties en dat de data de juiste types heeft. De laag zelf bevat vooral code om te zorgen dat de logicalaag makkelijk specifieke data op kan halen.

### Database

Als database gebruik ik SQL Server. Dat komt omdat ik er redelijk veel ervaring mee heb, en Fontys een gratis
host hiervoor aanbiedt. Verder maakt het niet heel veel uit welke database ik gebruik, omdat dit achter EFCore (als ORM)
zit. Hierdoor kan ik de data eenvoudig vanuit C# uitlezen en is het niet nodig om SQL te schrijven. Ook heeft EFCore
migraties waardoor ik verzekerd ben dat de database klopt en de gegevens veilig blijven.