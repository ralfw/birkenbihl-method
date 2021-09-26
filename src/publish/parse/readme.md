# Parse

## Parse into sentences

First parse the text into sentences because they are at the hear of the interlinear presentation.
Text is translated sentence by sentence.

```
# Дядо Коледа

# Джудженцето Алфи

Фабриката за играчки на Дядо Коледа се_намира на Северния полюс и там декември винаги е най-натоварения месец.

Така беше и тази година.

Дядо_Коледа непрекъснато обикаляше фабриката.

Трябваше да е сигурен, че всичко е наред.

Джудженцата работеха здравата.
Всяко си имаше задача.

// ...
```

Every line a sentence. A blank line separates paragraphs.

This format can easily be checked for malformed sentences.

Source file `abc.md` becomes `abc.sentences.md`.


## Parse into words

Translation is done sentence by sentence in textual and verbal form. In addition a word-by-word translation is needed.
If words in a sentence should not be separated, they can be bound to each other with a `_`.

```
Дядо
Коледа


Джудженцето
Алфи


Фабриката 
за 
играчки
на
Дядо
Коледа 
се намира
на
Северния 
полюс
и
там
декември 
винаги
е
най-натоварения
месец.


Така
беше
и
тази
година.


Дядо_Коледа
непрекъснато
обикаляше 
ABC:фабриката.
//...
```

One word each line. Sentences separated by a single blank line; two blank lines between paragraphs.

Any punctuations characters are still attached to the words.

Any `_` in a word will be replaced with a space.

File `abc.sentences.md` becomes `abc.words.txt`.

When rendering the text with translations it will be re-generated from this file.


## Compile index

A translation has only to be done for unique words. Hence a compilation of unique words, an index, is needed:

```
Дядо
Коледа
Джудженцето
Алфи
Фабриката 
за 
играчки
на
се намира
Северния 
полюс
и
там
декември 
винаги
е
най-натоварения
месец
Така
беше
тази
година
Дядо Коледа
//...
```

One word on each line. The list is case sensitive since.

The index file would be `abc.index.csv`.

### Index with translations

Translations can be attached to each word after a `;`.

```
Дядо;Grandpa
Коледа;Christmas
Джудженцето;Dwarf
Алфи;Alfi
Фабриката;the factory 
за;for
играчки;toys
на;of
се намира;is located
Северния;northern
полюс;pole
и;and
там;there
декември;December
винаги;always
е;is
най-натоварения;most busy
месец;month
Така;So
беше;it was
тази;this
година;year
Дядо Коледа;Santa Claus
//...
```

#### Immutable translations

A translation that should not be replaced in case of an automatic re-translation could be denoted with `\`.

```
//...
Северния;\North
//...
```

### Meta data

Also a number of occurrenes could be given for eacht word, eg.

```
Дядо;Grandpa;10
Коледа;Christmas;10
Джудженцето;Dwarf;7
Алфи;Alfi;4
//...
```

And each word could be tagged to link it to a certain context:

```
фабриката==Santa Claus' factory;1;ABC
```

The tag could be applied to the words file like this: `<tag> ":" word`.

### Structure of index file

| Original word | Translation | Number of occurrences | Tag |
| ------------- | ----------- | --------------------- | --- |
| Дядо | Grandpa | 10 | |

As a CSV file it can easily be opened in Excel, sorted, changed, exported.

Should an original or a translation contain the delimited it can be set in `"`.