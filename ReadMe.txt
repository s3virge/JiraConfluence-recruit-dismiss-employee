Режим отладки не работает. Вываливается исключения что не найден элемент, но если запустить релиз, то все в порядке.
Отлавливал баги с помощью log4net.
log4net.config лежит в проекте JiraConfluence recruit-dismiss employee
логирование включается в файле AssemblyInfo.cs этой командой [assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
Разукрашивание вывода не работает, не разобрался почему.
Логирование отключается если закоментить строку в AssemblyInfo