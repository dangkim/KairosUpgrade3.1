### CLI

server: 77.57

path:  X:\Consolidate-Data

-DATE `yyyy-MM-dd  GMT+8`

-OPERATOR `operator id`

-Game `game id`

-METHOD `1 | 2`

  - 1: all in one time   
  - 2: per hour

-TARGET `1 | 2 | 4 | 7`

  - 1 platform report
  - 2 conindenomination
  - 4 tournamentreport
  - 7 all

-TOURNAMENT `tournament id`

### examples

for one operator with all reports

`Slot.WindowsService.exe -METHOD 2 -TARGET 7 -OPERATOR 1 -DATE 2019-01-23`

fro all operators with all reports

`Slot.WindowsService.exe -METHOD 2 -TARGET 7 -DATE 2019-01-23`

