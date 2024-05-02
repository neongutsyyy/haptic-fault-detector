import json
import os

def combine_json_files(input_dir, output_file):
    all_data = []

    for filename in os.listdir(input_dir):
        if filename.endswith(".json"):
            filepath = os.path.join(input_dir, filename)
            with open(filepath, 'r', encoding='utf-8') as f:
                file_data = json.load(f)
                all_data.extend(file_data) 

    with open(output_file, 'w', encoding='utf-8') as outfile:
        json.dump(all_data, outfile)


input_dir_fault = r"C:\Users\amitm\VibrationAnalysis\faulty_data"
output_file_fault = r"C:\Users\amitm\VibrationAnalysis\ml\combined_data_fault.json"
combine_json_files(input_dir_fault, output_file_fault)

input_dir_okay = r"C:\Users\amitm\VibrationAnalysis\okay_data"
output_file_okay = r"C:\Users\amitm\VibrationAnalysis\ml\combined_data_okay.json"
combine_json_files(input_dir_okay, output_file_okay)



