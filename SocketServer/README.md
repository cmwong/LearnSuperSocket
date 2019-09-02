# Implement custom FixedHeader

* FixedHeader 8 bytes into 4 uint16
* 1st value = ? (unknow usage. it is reserve engineering from a c++ code.)
* 2nd value = message length (including header and body)
* 3rd value = mainKey
* 4th value = subKey
