# SelfDecryptingImage

Encrypted image with AES and embed it into HTML file, which displays original image if proper key is supplied

# Purpose

Image files, such as screenshots and photos often contain personal information. It is important to
share them in a manner, which prohibits the ISP from intercepting the images. There are services, which
do store data encrypted, but the files are then decrypted on request by server, thus disclosing encryption
key. This project strives to solve the problem of storing images securely by making decryption key a hashtag,
which is not sent to remote server when requesting resources.

As a result, users could be securely swapping images by sending URLs, which could look like this:

    https://hostname.com/host/file.html#Yc6XHEWmaZQcEzajGab63pWLUsy7ucQ4Wz1hiWtdb3s=

When requested on the server, only the following part will be sent:

	https://hostname.com/host/file.html

thus concealing file decryption key.

# Compiling

The project is built using monodevelop and requires Mono/.NET 4.5 framework. It is possible to build the project 
using Visual Studio 2012 and higher.

# Tools supplied

## makesdi

convert jped or png file into a self-decrypting HTML. Resulting file has necessary libraries embedded to
decrypt embedded image in browser, if a proper key and iv are supplied on the URL line

## postsdi

will post the supplied jpeg or png file to google drive and share the folder, making file accessible from the
Inrernet. One usage scenario could be using it with in conjunction with screenshot application, allowing easy 
and secure sharing of screenshots.

# Acknowledgements

These tools contains code from following projects:

* [Google Apis](https://developers.google.com/gdata/client-cs)
* [SlowAES](http://code.google.com/p/slowaes/)
* [base64-js](https://github.com/beatgammit/base64-js)
	

