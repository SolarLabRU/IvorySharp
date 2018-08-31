# IvorySharp

[![Build status](https://travis-ci.org/SolarLabRU/IvorySharp.svg?branch=master)](https://travis-ci.org/SolarLabRU/IvorySharp)
[![Build status](https://ci.appveyor.com/api/projects/status/1ojaoq8ew6y2jvpn/branch/master?svg=true)](https://ci.appveyor.com/project/rex-core/ivorysharp/branch/master)
[![NuGet version](https://img.shields.io/nuget/v/IvorySharp.svg)](https://www.nuget.org/packages/IvorySharp/)

> Платформа: NET Standard 2.0 |
```diff
На данный момент документация в Wiki неактуальна. Обновление в скором времени.
```
Библиотека предоставляет набор компонентов, реализующих некоторые возможности парадигмы [АОП](https://ru.wikipedia.org/wiki/Аспектно-ориентированное_программирование) в языке C#. Если точнее, то библиотека позволяет вынести сквозную функциональность в отдельные компоненты и декларативно применять их к методам бизнес логики, используя атрибуты.


## И зачем мне это ваше АОП ?
Как уже отмечалось, применений идей парадигмы позволяет избавиться от дублирующегося кода, разбросанного по всем слоям приложения. Примером может стать обработка ошибок. Допустим, есть ряд репозиториев, расположенных на уровне доступа к данным.
```C#
interface IUserRepository 
{
    int Add(User user);
    int Update(User user); 
}

```
В каждом методе репозитория находится одинаковая обработка ошибок вида
```C#
class UserRepository : IUserRepository 
{
  int Add(User user)
  {
    try {
      // Добавление в базу пользователя
    } catch (Exception e) {
      Logger.Error(e.Message, e);
      throw new WrappedException(e);
    }
  }
}

```
Рано или поздно обилие однообразных фрагментов кода приводит к появлению обобщенных обработчиков вида
```C#
class ExceptionHandlers {

  public static T HandleExceptions<T>(Func<T> action) 
  { ... }

}
```
И репозиторий преображается в следующий вид
```C#
class UserRepository : IUserRepository 
{
  int Add(User user)
  {
    return ExceptionHandlers.HandleExceptions(() => 
    { 
      // Добавление пользователя
    });
  }
}
```
Однако, это не решает проблему и так же нарушает принцип [DRY](https://ru.wikipedia.org/wiki/Don’t_repeat_yourself), так как для отличающихся по сигнатуре делегатов придется писать свой дублирующийся фрагмент кода. При этом комбинирование такого рода обработчиков сводит читаемость и удобство поддержки функциональности на нет. В варианте с применением АОП репозиторий преобразуется следующим образом

```C#
interface IUserRepository 
{
    [HandleExceptionAspect]
    int Add(User user);
}

class UserRepository : IUserRepository {
  int Add(User user) {
    // Добавление пользователя
  }
}

[AttributeUsage(AttributeTargets.Method)]
class HandleExceptionAspect : MethodBoundaryAspect 
{
  public override void OnException(IInvocationPipeline pipeline)
  {
    var exception = pipeline.CurrentException;
    Logger.Error(exception.Message, exception);
    
    // Выбросит исключение, сохранив стек-трейс
    pipeline.Throw(new WrapperException(exception));
  }
}

```   
При этом возможно применение нескольких аспектов, в том числе аспектов верхнего уровня на интерфейсах без потери читаемости и поддерживаемости бизнес логики. Детализированные примеры аспектов и их использования представлены [тут](https://github.com/rex-core/IvorySharp/tree/master/src/IvorySharp.Examples).

## И никто раньше такого не делал?
Делали само собой. Наиболее популярная библиотека реализующая АОП  - это [PostSharp](https://www.postsharp.net). Его функциональность гораздо шире этой библиотеки, однако есть один минус - он платный. Есть и бесплатные альтернативы, например [Fody](https://github.com/Fody/Fody) и [Mr.Advice](https://github.com/ArxOne/MrAdvice), однако у них другой принцип работы. Данная библиотека выполняет динамическое проксирование вызовов через стандартные механизмы платформы ([DispatchProxy](https://github.com/dotnet/corefx/blob/master/src/System.Reflection.DispatchProxy/src/System/Reflection/DispatchProxy.cs)), в то время как PostSharp и Fody выполняют [инъекцию IL кода](http://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil/) во время компиляции, что улучшает общую производительность, однако модифицирует код сборки. Наиболее близкое по принципу работы к представленной библиотеке - механизм динамического перехвата вызова в [Castle.DynamicProxy](https://github.com/castleproject/Core/blob/master/docs/dynamicproxy-introduction.md).
Для того чтобы лучше понять детали реализации библиотеки советую ознакомиться с [этой](https://msdn.microsoft.com/en-us/magazine/dn574804.aspx) статьей.

## И в чем тогда преимущества этой библиотеки?
При разработке основной упор делается на производительность, так что сейчас вызов скомпонованного с аспектами метода немногим уступает по скорости, чем [вызов метода через рефлексию](https://github.com/rex-core/IvorySharp/wiki/Быстродействие). Так же есть возможность использования нескольких аспектов на методе и гибкий механизм управления [пайплайном выполнения метода](https://github.com/rex-core/IvorySharp/wiki/Пайплайн-выполнения). Доступны плагины для интеграции со сторонними фреймворками по внедрению зависимостей и реализован механизм [внедрения зависимостей в аспекты](https://github.com/rex-core/IvorySharp/wiki/Внедрение-зависимостей).
