# OWASP top 10 verslag

## Samenvatting

| Naam                                           | Samenvatting                             | Bij mij | Fixen |
|------------------------------------------------|------------------------------------------|----------|--------|
| A01 Broken Access Control                      | Slechte controle op account privilege    | Nee      |        |  
| A02 Cryptographic Failures                     | Encryptiefouten                          | Nee      |        |  
| A03 Injection                                  | SQL injectie (ook voor ORMs)             | Nee      |        |  
| A04 Insecure Design                            | Fouten in de user flow of business logic | Ja       | Ja     |  
| A05 Security Misconfiguration                  | Onveilige instellingen                   | Deels    | Ja     |  
| A06 Vulnerable and Outdated Components         | Kwetsbare oude versies                   | Nee      |        |  
| A07 Identification and Authentication Failures | Login beveiligingsfouten                 | Deels    | Deels  |  
| A08 Software and Data Integrity Failures       | Vertrouwen van externe bronnen           | Nee      |        |  
| A09 Security Logging and Monitoring Failures   | Geen of slechte logging                  | Ja       | Deels  |  
| A10 Server Side Request Forgery (SSRF)         | De server kan onveilige requests maken | Nee      |        |

## A01 Broken Access Control

Deze komt niet voor in mijn systeem, omdat ik voor de authenticatie asp.net core Identity gebruik. Dit is een standaard
authenticatie systeem dat al deze problemen al oplost. Bij ieder endpoint dat privé zou moeten zijn is de username al
bekend, en wordt gecontroleerd of de aangevraagde data toegankelijk is voor deze gebruiker.

## A02 Cryptographic Failures

Hier heb ik geen last van omdat asp.net core Identity al de encryptie van wachtwoorden en andere data regelt.

## A03 Injection

Dit is ook geen probleem, omdat Entity Framework Core zelf SQL-injectie voorkomt.

## A04 Insecure Design

Waarschijnlijk is dit geen probleem, maar ik heb dit nog niet gecontroleerd. Ook zijn er nog niet voor alle edge-cases
tests geschreven. Dit ga ik zo snel mogelijk doen.

## A05 Security Misconfiguration

De standaard veiligheidsinstellingen zijn laten staan. Ook is er geen standaard admin wachtwoord. Wel is het zo dat als
de logic een probleem heeft de hele stack trace naar de client wordt gestuurd als de response body. Dit kan onveilig
zijn, en ga ik dus oplossen.

## A06 Vulnerable and Outdated Components

Zelf controleer ik of versies achterlopen, en daarnaast gebruik ik Dependabot om dit automatisch te doen. Dit is dus
geen probleem.

## A07 Identification and Authentication Failures

EFCore slaat wachtwoorden veilig op. Daarnaast controleer ik in de frontend en backend of het wachtwoord sterk genoeg
is. Er is alleen nog geen rate-limiting ingebouwd. Dit kan makkelijk verholpen worden. Twee-factor authenticatie is ook
nog niet geïmplementeerd, maar dit is veel meer werk, en heeft nu dus geen prioriteit.

## A08 Software and Data Integrity Failures

Hier heb ik niet mee te maken, omdat ik alle libraries in de build inbouw. Verder gebruik ik ook geen externe data.

## A09 Security Logging and Monitoring Failures

Op dit moment log ik niets. Als er iets misgaat met de beveiliging heb ik dus geen enkel idee van de context. Om dit op
te lossen moet ik op meerdere plekken logging inbouwen. Dit is niet moeilijk, maar wel veel werk. Daarom begin ik met de
eenvoudigere plekken te loggen.

## A10 Server Side Request Forgery (SSRF)

Mijn server maakt geen requests naar andere servers, dus dit is geen probleem.