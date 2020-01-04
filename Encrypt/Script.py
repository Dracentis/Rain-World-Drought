
fileID = 77

while (fileID < 97) :
	file = open(str(fileID)+".txt", "a+")
	file.write("\n")
	file.close()
	fileID = fileID + 1
	