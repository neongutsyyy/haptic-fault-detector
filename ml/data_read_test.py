import serial
import json

ser = serial.Serial('COM5', baudrate=31250)

data_list = []  

for i in range(200):  
    data_bytes = ser.readline()  
    data_str = data_bytes.decode('utf-8').strip()  
    x, y, z = map(int, data_str.split(',')) 
    print(x, y, z) 
    data_tuple = (x, y, z)  
    data_list.append(data_tuple)  

json_data = [list(data_point) for data_point in data_list]

# Save data to a JSON file
with open('sdata_notokayne1.json', 'w') as json_file:
    json.dump(json_data, json_file)

print("Data saved to sensor_data.json")

