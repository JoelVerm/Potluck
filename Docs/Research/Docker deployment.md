# Docker deployment

Docker wordt vaak gebruikt om ingewikkelde systemen te deployen. Zeker het gebruik van Docker compose helpt hierbij, omdat het hele systeem na één commando online is. Maar je moet nog wel dat commando starten, en het is niet erg snel... Zijn er betere manieren?

## Onderzoeksvraag

Wat is de meest geschikte manier om docker te gebruiken voor het deployen van een systeem met meerdere containers zoals Potluck?

Hierbij komen meerdere subvragen kijken:

- Welke manieren zijn het meest populair?
- Welke manier is het meest geschikt?

## Welke manieren zijn het meest populair?

Om deze vraag te beantwoorden zijn de volgende methodes van toepassing: Community research en Literature study.

### Community research

Op forums zoals StackOverflow worden allerlei alternatieven aangeboden op Docker compose. Hier zijn de meest voorkomende aanbevelingen uitgezocht.

### Literature study

De aanbevelingen van onbekende mensen op forums lopen erg uiteen, daarom zijn die gefilterd op twee criteria:

- Heeft deze oplossing met Docker te maken?
- Wordt deze oplossing vaker genoemd?

Daarnaast is gebruik gemaakt van de StackOverflow survey (1) om te bepalen wat de meest populaire opties zijn en een volgorde aan te brengen.

### Resultaat

De gevonden mogelijke opties waren op volgorde van populariteit:

- Kubernetes
- Docker stack
- Docker compose
- HashiCorp Nomad
- Podman

## Welke manier is het meest geschikt?

De lijst met mogelijkheden bevat verschillende soorten tools. Die zijn niet allemaal geschikt voor Potluck. Daarom is de beste optie uitgekozen. Voor deze vraag is ook Literature study gebruikt. Daarna zijn de twee meest geschikte opties vergeleken met Multi-criteria decision making.

### Literature study

De opties die naar voren kwamen zijn gefilterd op geschiktheid door het lezen van de betreffende documentatie. Hierbij waren de belangrijkste criteria eenvoud en functionaliteit. Oplossingen die te ingewikkeld zijn, zijn minder geschikt voor een eenvoudiger systeem zoals Potluck. Wel heeft het genoeg functionaliteit nodig om in één keer het hele systeem te kunnen starten.

#### Resultaat

De gevonden mogelijke opties waren:

- Kubernetes
- Docker stack
- Docker compose
- HashiCorp Nomad
- Podman

Kubernetes (2) is vooral ontworpen voor grote bedrijven. Het kan goed overweg met meerdere systemen met miljoenen gebruikers. Daardoor is het wel veel complexer dan nodig is. Daarom viel het hier af.

Docker stack (3) lijkt heel erg op Docker compose. Het gebruikt ook een compose.yaml, maar is flexibeler, en gaat ervan uit dat het op een productie machine draait. Dit lijkt op wat hier nodig is.

Docker compose (4) is hetgeen waarvoor alternatieven gezocht worden. Ondanks dat lijkt het nog steeds een goede oplossing, omdat het wel de gewenste functionaliteit bevat.

Nomad (5) lijkt erg op Kubernetes. Het is eenvoudiger te leren en gebruiken, maar nog steeds gemaakt om grotere systemen te runnen, en meer werk dan nodig voor kleinere applicaties.

Podman (6) kan erg veel maar is nog steeds eenvoudig. Helaas bleek het niet gespecialiseerd in complete systemen, maar beter in losse containers. Dit is niet wat gezocht wordt en Podman viel dus af.

Uiteindelijk zijn Docker stack en Docker compose geselecteerd als mogelijke oplossingen.

### Multi-criteria decision making

Om te bepalen welke manier het meest geschikt is, moet er een uiteindelijke keuze gemaakt worden uit de twee opties, Docker stack en Docker compose. Daarvoor zijn de volgende voorwaarden van belang:

1. Schaal van gebruik: Werkt het voor single-node of multi-node setups? Potluck gebruikt een single-node setup.
2. Gebruiksgemak: Hoe makkelijk is het lokaal te gebruiken?
3. Handige functies: Biedt het automatische herstart, iteratieve updates en load balancing? Zeker de eerste twee zijn nuttig om te hebben.
4. Prestaties: Hoeveel extra belasting geeft het?
5. Flexibiliteit: Werkt het voor verschillende workflows?
6. Geschikt voor productie: Is het betrouwbaar voor productieomgevingen?

#### Vergelijkingstabel

| **Criterium**             | **Docker Stack**                               | **Docker Compose**                              |
|---------------------------|-----------------------------------------------|-----------------------------------------------|
| **Schaal van gebruik**    | Gemaakt voor multi-node setups (Swarm mode).  | Werkt perfect voor single-node setups.        |
| **Gebruiksgemak**         | Complexer; Swarm-configuratie nodig.          | Simpel en intuïtief, vooral lokaal.           |
| **Handige functies**      | Automatische herstart, iteratieve updates en ingebouwde load balancing. | Alleen basisfuncties; handmatig bijwerken.   |
| **Prestaties**            | Iets meer belasting door Swarm-processen.     | Lichtgewicht; geen extra processen.           |
| **Flexibiliteit**         | Gericht op productieclusters en schaling.     | Beter voor lokale en eenvoudige setups.       |
| **Geschikt voor productie**| Ideaal voor professionele productieomgevingen. | Minder geschikt voor high-availability toepassingen. |

#### Resultaat
Potluck is een single-node project, maar automatische herstart en iteratieve updates zijn erg handig. Ook de toekomstgerichtheid is belangrijk: altijd kunnen opschalen als dat nodig is, en makkelijk professionele functies kunnen gebruiken. Op deze punten blinkt Docker stack uit.

Aan de andere kant is Docker compose een stuk eenvoudiger. Het kan niet omgaan met grote systemen, maar het is makkelijker om lokaal het hele systeem online te krijgen.

## Conclusie

Van de meest populaire manieren zijn Docker compose en Docker stack het meest geschikt. Terwijl om lokaal te ontwikkelen en testen Docker compose erg handig is, is voor het deployen van het systeem Docker stack duidelijk het beste. Daarom is Docker stack de meest geschikte manier voor productiedeployment van systemen zoals Potluck.

## Bronnen

1. https://survey.stackoverflow.co/2024/
2. https://kubernetes.io/docs/concepts/overview/#why-you-need-kubernetes-and-what-can-it-do
3. https://docs.docker.com/engine/swarm/stack-deploy/
4. https://docs.docker.com/compose/intro/features-uses/
5. https://developer.hashicorp.com/nomad/tutorials/get-started/gs-overview
6. http://podman.io/docs