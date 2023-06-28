![Travel Agency](https://i.imgur.com/EWLDqI9.png)

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![MongoDB](https://img.shields.io/badge/MongoDB-%234ea94b.svg?style=for-the-badge&logo=mongodb&logoColor=white) ![HTML5](https://img.shields.io/badge/html5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white)

# Travel Agency

## Project goal

"Travel Agency", developed as our 3rd year college project, is a program used for class "Human-Computer Interaction" in 2023. Main goal of the project was to create a program for specific user with specific needs.

The main goal of the project was to test the practical knowledge in the field of interface modeling, documentation and interface revisions, and application development that uses direct manipulation and drag&drop technique in accordance with guidelines for good user interface development.

## User persona

The user we had to make program for was a 22 year old male, who has barely any knowledge about the program itself, as well as minimal computer knowledge. The user is **color blind**, and mainly relies on using "online" systems of help.

The program in this scenario is supposed to handle huge amounts of data, so filtering and search is necessary wherever possible.

## Project structure

The whole program consists of travels, which can include restaurants, attractions and accommodations. Each travel has a distance (between each attraction), price and location count.  Travels can be ordered at any time user wishes.

Speaking of users, there's 2 types of people using the program: **regular user** and **agent**. 

Regular user is able to book a travel, see currently available travels and check their travel history. Agent is able to create, view, delete and edit currently available travels, along with locations included with them, as well as check stats about sold travels.

**Map API** that we used for this project was Bing Maps. With it, we can use basic functions like zoom in, zoom out, drag the map, as well as dropping pins and routes between pins!

## Handling data

For the database, we used **MongoDB**. We were already familiar with the program itself, and for smaller projects like this, it's easily usable and accessible. The user credentials weren't hashed, since the main focus of the project wasn't to protect user data.

The project itself included filtering (by price, by distance, etc.). It was mainly used on windows like "Checking individual travel sales".

![Travel Agency](https://i.imgur.com/JYIOsjH.png)

## Help

Lastly, project required "Help" system, so we included help pop-up with the press of "F1". Depending on the current window, user could see what each button on the page does, as well as navigate to the help for other pages in the program itself.

![Travel Agency](https://i.imgur.com/o5aTpgB.png)
