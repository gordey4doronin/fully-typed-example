en-US :us: | [ru-RU :ru:](https://github.com/gordey4doronin/fully-typed-example/blob/master/README.ru-RU.md)
# On the way to full typing with TypeScript, Swashbuckle, and AutoRest

## Introduction

The article is devoted to the question of implementing exchange of typed messages between Back-End based on ASP.NET Web API and Front-End created with the use of TypeScript.
It is important for the teams working on big projects, especially for the distributed ones.
For example, when Back-End and Front-End developers work from different places and time zones often lacking a chance to communicate and discuss any issue.
Tracking changes in this case can be rigorous and causing elusive errors sometimes.

One of the main problems the author (who came to Front-End from WPF and Silverlight) faced was the absence of static typing.
Quite often instead of adding “2” and “2” he added “2” and “Callback function returning 2”, or passed DOM object instead of its jQuery wrapper.
Of course, occurrence of such static code analyzers as JSLint served as a solution for this problem.
However, TypeScript was a real breakthrough, especially for the teamwork.

![TypeScript + Swagger logo](https://habrastorage.org/files/b0b/c02/9aa/b0bc029aa8f7457784c62d25b8dbb42b.png)
