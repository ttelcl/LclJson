# JsonReformat

JSON reformatting tool. 

## Features:

- Reformat a JSON file to indented form
- Reformat a JSON file to flat form
- Reformat a JSON file to a mixed indented / flat form,
  according to your specified rules. Rules include:
  - Switch to flat form beyond a specified nesting depth
  - (other rules are still in design)


## Examples

* Output a fully indented version of "mydata.json" as "mydata.out.json"
  ```bash
  jsonreformat -i mydata.json
  ```
* Output a fully indented version of "mydata.json" as "foobar.json"
  ```bash
  jsonreformat -i mydata.json -o foobar.json
  ```
* Output a flattened (unindented) version of "mydata.json" as "mydata.out.json"
  (both variants are equivalent)
  ```bash
  jsonreformat -i mydata.json -flat
  ```
  ```bash
  jsonreformat -i mydata.json -flvl 0
  ```
* Output a version of "mydata.json" where only the top level is indented
  ```bash
  jsonreformat -i mydata.json -flvl 1
  ```
* Output a version of "mydata.json" where only the top two levels are indented
  ```bash
  jsonreformat -i mydata.json -flvl 2
  ```
