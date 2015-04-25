##SimpleStorage

###Описание
SimpleStorage - очень простая база данных, которая все хранит в памяти.
На ее примере можно изучить, как устроены внутри вполне реальные хранилища вроде MongoDB или Cassandra.

###Внутреннее устройство
С SimpleStorage можно взаимодействовать через несколько интерфейсов:

####[ValuesController.cs](SimpleStorage/SimpleStorage/Controllers/ValuesController.cs):

* Get (id) - позволяет получить значение по ключу. Если ключ неизвестен, то в ответ возвращается `404 Not Found`.
* Put (id, value) - позволяет записать значение по ключу.

####[AdminController](SimpleStorage/SimpleStorage/Controllers/AdminController.cs):

* GetAllLocalData - позволяет получить все данные, которые есть на узле, к которому обращаемся.

###FAQ
Если во время запуска тестов возникнет ошибка "Доступ запрещен", значит, приложению запрещено "слушать" нужные порты. Есть несколько способов, как избавиться от подобной ошибки:

* Запустить VisualStudio с правами администратора
* Выполнить команды:
  ```
  netsh http add urlacl url=http://+:15000/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15001/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15002/ user=Everyone listen=yes
  ```
  Если у вас "русская" Windows, то может помочь заменить `user=Everyone` на `user=Все`.
