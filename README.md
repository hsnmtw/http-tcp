# http-tcp

This web server is not very special, but comes with the following concepts:

1. Every page request should be independant of others, therefore, we don't use shared application memory.
2. The server is a Host, not container. This means that the server hosts some files, may execute some binaries and get the output and push it as is to the client.
3. Defined responsibility: the server responsibility lies with serving files/web pages only. Processing of data, fetching data, calculating results, generating reports is a service that is delivered via binaries which the server help in executing.
4. The aim is to have a modular web app concept, something like the linux operating system where you can use an executable that generates some result that can be piped to another executable and so forth. For example:

                        $> ls - ltr | grep something | wc -l
                    

    ls -ltr : lists some files in current directory

    grep : finds only lines with match string

    wc -l : word count by line
    which tells you how many files exists in the current directory that contains string 'something'

5. Currently, binaries can be executed individually, however, I am still working on an easy way to make the server route some actions to collection of binaries with some defined order
