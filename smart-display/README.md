## Setup a Google Assistant for Google Nest Hub:

Follow instructions outlined in the [no-ui sample](https://github.com/emmanueltissera/umbraco-heartcore-omnichannel/tree/master/no-ui) to set this up.

The secret to a rich response is this snippet of code:

```
agent.add(new Card({
  title: specialTitle,
  imageUrl: specialImage,
  text: specialMessage
}));
```