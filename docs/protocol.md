# Communication Protocol Description

## Serial Communication Parameters

- **Baud rate:** 9600 bps
- **Data bits:** 8
- **Parity:** None
- **Stop bits:** 1
- **Slave addressing (RS485):** 1-99

**Note:** For RS232 communication, the slave address needs to be set to ```0x01```. The protocol should be the same for both RS485 and RS232.


## Read Commands

### Byte Frame (5 Bytes)

  | Byte Position | Function            |
  | ------------- | ------------------- |
  | 0             | Header (=```0x00``` |
  | 1             | Slave Address       |
  | 2             | Command             |
  | 3             | Block Number        |
  | 4             | Checksum            |


### Read Error Memory (```0x01```)
  - **Block Number:** 1-5 (2 errors per block) -> 10 memory slots in total
  - **Response:**
    | Byte Position | Function          | Conversion        |
    | ------------- | ----------------- | ----------------- |
    | 0             | Month             | to decimal        |
    | 1             | Day               | to decimal        |
    | 2             | Hour              | to decimal        |
    | 3             | Minute            | to decimal        |
    | 4             | Error Code        | *unknown*         |
    | 5             | Year (since 2000) | to decimal        |
    | 6-11          | Next Error        |                   |
    | 12            | Checksum          |                   |


### Read Online Values (```0x02```)
  - **Block Number:**
      - Single-Phase Inverters (NT5000): 1
      - Three-Phase Inverters (e.g. NT10000): 1-3 (one block per phase)
  - **Response:**
    | Byte Position | Function          | Conversion   | Calculation                          | Unit   |
    | ------------- | ----------------- | ------------ | ------------------------------------ | ------ |
    | 0             | UDC (DC voltage)  | to decimal   | buffer[0] * 2.8 + $udc-offset        | V      |
    | 1             | IDC (DC current)  | to decimal   | buffer[1] * 0.08                     | A      |
    | 2             | UAC (AC voltage)  | to decimal   | buffer[2] + 100                      | V      |
    | 3             | IAC (AC current)  | to decimal   | buffer[3] * 0.12                     | A      |
    | 4             | Temperature       | to decimal   | buffer[4] - 40                       | °C     |
    | 5             | Heat Flux         | to decimal   | buffer[5] * 6                        | W/m^2  |
    | 6, 7          | Energy Today      | to decimal   | (buffer[6] * 256 + buffer[7]) / 1000 | kWh    |
    | 8, 9          | Energy Total      | to decimal   | (buffer[8] * 256 + buffer[9]) / 1000 | kWh    |
    |               | PDC (DC power)    |              | (UDC * IDC) / 1000                   | kW     |
    |               | PAC (AC power)    |              | (UAC * IAC) / 1000                   | kW     |
    | 12            | Checksum          |              |                                      |        |

    **Note:** If the temperature or heat flux sensor is not connected, the buffer value is ```0x01```.

    **$udc-offset:**
      - Single-Phase Inverters: 100
      - Three-Phase Inverters: 200


### Read Day Values (```0x03```)
  *not yet implemented*


### Read Month Values (```0x04```)
  *not yet implemented*


### Read Minute Values (```0x05```)
  *not yet implemented*
  

### Read Date + Time (```0x06```)
  - **Block Number:** 1
  - **Response:**
    | Byte Position | Function          | Conversion        |
    | ------------- | ----------------- | ----------------- |
    | 0             | Year (since 2000) | to decimal        |
    | 1             | Month             | to decimal        |
    | 2             | Day               | to decimal        |
    | 3             | Hour              | to decimal        |
    | 4             | Minute            | to decimal        |
    | 12            | Checksum          |                   |


### Read Serial Number (```0x08```)
  - **Block Number:** 1 (2 is reserved for longer serial numbers) 
  - **Response:**
    | Byte Position | Function          | Conversion        |
    | ------------- | ----------------- | ----------------- |
    | 0-11          | Serial Number     | to ASCII String   |
    | 12            | Checksum          |                   |

    ```0x0D``` is a filling byte and can be ignored.


### Read Protocol + Firmware Version (```0x09```)
  - **Block Number:** 1
  - **Response:**
    | Byte Position | Function          | Conversion        |
    | ------------- | ----------------- | ----------------- |
    | 0-1           | Protocol Version  | to ASCII String   |
    | 2-11          | Firmware Version  | to ASCII String   |
    | 12            | Checksum          |                   |

    ```0x0D``` is a filling byte and can be ignored.
    

## Write Commands

### Byte Frame (5 Bytes)

  | Byte Position | Function                           |
  | ------------- | ---------------------------------- |
  | 0             | Header (=```0x00```)               |
  | 1             | Slave Address (0xFF for Broadcast) |
  | 2             | Command                            |
  | 3             | Value                              |
  | 4             | Checksum                           |

  The inverter returns no response to write commands.


### Set Year (```0x32```)
  
  **Note:** 2000 needs to be subtracted from the actual year (e.g. 2026 - 2000 = 26)
  

### Set Month (```0x33```)


### Set Day (```0x34```)


### Set Hour (```0x35```)

  **Note:** 1 needs to be added to the actual hour value


### Set Minute (```0x36```)

  **Note:** 1 needs to be added to the actual minute value



## Checksum Calculation

**Checksum = (sum of all data bytes) % 256**  
Only the checksum byte itself is excluded from the sum.  



