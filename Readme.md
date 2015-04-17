##SimpleStorage

###Описание
SimpleStorage - очень простая база данных, которая все хранит в памяти.
На ее примере можно изучить, как устроены внутри вполне реальные хранилища вроде MongoDB или Cassandra.

###Внутреннее устройство
SimpleStorage умеет выполнять следующий набор операций:

###FAQ
Если во время запуска тестов возникнет ошибка "Доступ запрещен", значит, приложению запрещено "слушать" определенные порты. Есть несколько способов, как избавиться от подобной ошибки:

* Запускать VisualStudio с правами администратора
* Выполнить команду:
  ```
  netsh http add urlacl url=http://+:15000/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15001/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15002/ user=Everyone listen=yes
  ```
