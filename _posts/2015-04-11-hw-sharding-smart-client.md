---
layout: post
title:  "Домашняя работа №2: Умный клиент"
categories: Sharding, Smart Client
date:   2015-04-11
---

Нужно будет реализовать схему "Умный клиент": сделать шардинг на клиенте.

Весь код можно взять [отсюда](https://github.com/ar7z1/courses). Там же есть очень краткое описание проекта.

Изменения нужно будет вносить в [SimpleStorageClient](https://github.com/ar7z1/courses/blob/master/SimpleStorage/Client/SimpleStorageClient.cs). Текущая реализация отправляет все запросы на первую шарду (нагрузка никак не балансируется). Чтобы нагрузка балансировалась, нужно по id вычислять, на какую шарду нужно писать данные. Аналогично нужно поступать и при чтении.

Проверить правильность решения нужно при помощи [ShardingTests](https://github.com/ar7z1/courses/blob/master/SimpleStorage/SimpleStorage.Tests/ShardingTests.cs) (предварительно удалив атрибут `[Ignore]`).

Решения нужно отправлять на [почту](mailto:art@skbkontur.ru).
