# Simple Binary Message Encoding Scheme

This project implements a simple binary message encoding scheme to be used in a signaling protocol for real-time communication applications. The encoding scheme allows the transmission of messages between peers, consisting of headers (name-value pairs) and a binary payload.

## Assumptions

The implementation of this project is based on the following assumptions:

1. The project exclusively focuses on encoding and does not encompass encryption functionalities.
2. Besides specifying the maximum number of headers, it is also assumed that a message can have no headers at all.
3. Only built-in exceptions are utilized within the project. If API consumers require more detailed error information, custom exceptions can be implemented.
4. The project aims to design a minimal binary encoding scheme specifically tailored for this use case. All limitations are declared as constants within the library. In a typical implementation, these limitations can be passed as settings to the class object implementing the 'MessageCodec' interface. For improved flexibility, dependency injection can also be employed.

## Encoding
The Encode function converts a Message object into a binary representation. The encoding process includes the following steps:

The first byte of the encoded message represents the header count. As the maximum header count is 63, a single byte is sufficient.
Each header name and value is encoded sequentially, with a two-byte prefix indicating their size. Since the maximum size for header names and values is 1023 bytes, two bytes are allocated for size representation.
The payload is appended to the end of the byte array without modification.

## Decoding
The Decode function converts a binary representation back into a Message object. The decoding process involves reversing the encoding steps and reconstructing the headers and payload from the provided byte array.

## Usage
To use the binary message encoding scheme in your application, follow these steps:

Add the project reference or package to your solution.
Instantiate the MessageCodec interface to encode and decode messages.
Create a Message object with the desired headers and payload.
Use the Encode function to convert the Message object into a byte array.
Transmit the byte array to the destination.
At the receiving end, use the Decode function to convert the byte array back into a Message object.
Extract the headers and payload from the decoded Message object for further processing.

## Running Unit Tests
The project includes unit tests to verify the correctness of the encoding and decoding functionality. To run the unit tests:

Open the project solution in Visual Studio.
Build the solution to ensure all dependencies are resolved.
In the Test Explorer window, select the tests you want to run or click "Run All" to run all tests.
Review the test results in the Test Explorer window and the output window.

