# Parse

## Project structure

### Paragraphs

Transform `.md` file into a *Birkenbihl Method* project (`.bmproj.json`).

In the project file all paragraphs and all sentences in those paragraphs 
are separated.

```
{
  paragraphs: [
    {
        sentences: [
            {
                text: "Дядо Коледа",
                headlinelevel: 1
            }
        ]
    },
    ...
    {
        sentences: [
            {
                text: "Пади и Пип работеха на машината за играчки."
            },
            {
                text: "Пади четеше названието на играчката и името на детето."
            },
        ]
    },
  ]
}
```

Whole paragraphs can easily be rebuild from sentences. But at the core of the Birkenbihl Method
is the interlinear display of sentences; that's why the first step is to extract them.

### Words

A following step might be to "once and for all" separate the sentences into words.
Because the interlinear text shows sentences word-by-word in the original and the translation.

```
{
  paragraphs: [
    {
        sentences: [
            {
                text: "Дядо Коледа",
                headlinelevel: 1,
                words: [ 
                    {
                        text: "Дядо"
                    },
                    {
                        text: "Коледа"
                    }
                ]
            }
        ]
    },
    ...
    {
        sentences: [
            {
                text: "Пади и Пип работеха на машината за играчки.",
                words: [
                    {
                        text: "Пади"
                    },
                    {
                        text: "и"
                    },
                    {
                        text: "Пип"
                    },
                    {
                        text: "работеха"
                    },
                    ...
                ]
            },
            ...
        ]
    },
  ]
}
```

Words of sentences contain punctuation characters.

### Index

Word-by-word translation should be done only for unique words. Hence a word index is
needed. That is generated from the words of the sentences.

```
{
    paragraphs: [
        ...
    ],
    
    wordindex: [
        {
            text: "Дядо"
        },
        {
            text: "Коледа"
        },
        ...
    ]
}
```

Words in the index don't contain punctuation characters.

A text might contain 500 paragraphs with 1500 sentences with 9000 words - 
but only 750 unique words.

## Solution idea

- Read `.md` file line by line.
    - If line starts with `//` then skip it
    - If line starts with `#` then
        - close current parapraph
        - open headline paragraph
            - add text of line as sentence
            - set headline level according to number of `#`
        - close headline paragraph
    - If line is empty
        - close current paragraph
    - else
        - open a new paragraph is none is open
        - split into sentences (detect `.?!`)
        - if a sentence is unfinished, append first sentence and finish it.
        - for all other sentences create a new one in the paragraph