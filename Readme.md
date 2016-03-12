##SimpleStorage

###Описание
SimpleStorage - очень простая база данных, которая все хранит в памяти.
На ее примере можно изучить, как устроены внутри вполне реальные хранилища вроде MongoDB или Cassandra.

###Внутреннее устройство
С SimpleStorage можно взаимодействовать через несколько интерфейсов:

####[ValuesController.cs](SimpleStorage/SimpleStorage/Controllers/ValuesController.cs):

* `Value Get(string id)` - позволяет получить значение по ключу. Если ключ неизвестен, то в ответ возвращается `404 Not Found`.
* `void Put(string id, [FromBody] Value value)` - позволяет записать значение по ключу.

####[ServiceController](SimpleStorage/SimpleStorage/Controllers/ServiceController.cs):

* `void Stop()` - остановить сервис.
* `void Start()` - запустить сервис.

####[OperationsController](SimpleStorage/SimpleStorage/Controllers/ServiceController.cs):

* `IEnumerable<Operation> Get(int position, int count)` - получить с нужной позиции нужное количество операций из oplog-а.

###FAQ
Если во время запуска тестов возникнет ошибка "Доступ запрещен", значит, приложению запрещено "слушать" нужные порты. Есть несколько способов, как избавиться от подобной ошибки:

* Запустить VisualStudio с правами администратора
* Выполнить команды:

  ```
  netsh http add urlacl url=http://+:15000/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15001/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15002/ user=Everyone listen=yes
  netsh http add urlacl url=http://+:15003/ user=Everyone listen=yes
  ```
  Если у вас "русская" Windows, то может помочь заменить `user=Everyone` на `user=Все`.
