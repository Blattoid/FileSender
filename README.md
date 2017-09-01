# FileSender
Allows sending files over the internet.

It works by reading the file, converting it to HEX, sending it over the internet to the destination defined in the .config. The receiver then reads the incoming data, saves it to a file, decodes it to binary data and saves it into the final file.

Complex? Yes. Clunky? Yes. Possibly can't handle files larger than 715MB? Yes. I created classes in case you want to use a certain part yourself. (Don't sue me if something breaks)
