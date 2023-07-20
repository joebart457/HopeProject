
#include "vm.hpp"
#include "ContextBuilder.hpp"
#include <conio.h>
#include <fstream>

int main(int argc, char** argv) {

	if (argc != 2) {
		std::cout << "usage: hope.exe <filepath>" << std::endl;
		return -1000;
	}
	std::string szFilePath = argv[1];

	vm machine(ContextBuilder().create_std_lib());

	std::ifstream file(szFilePath, std::ios::binary | std::ios::ate);
	if (file.is_open()) {

		std::streamsize size = file.tellg();
		file.seekg(0, std::ios::beg);

		std::vector<char> buffer(size);
		if (file.read(buffer.data(), size))
		{
			try {
				return machine.run(buffer.data(), size);
			}
			catch (std::exception ex) {
				std::cout << "err:" << ex.what() << std::endl;
				return -1001;
			}
		}
		std::cout << "cannot read binary data";
		file.close();
		return -998;
	}
	else {
		std::cout << "unable to open file " << szFilePath << std::endl;
		return -999;
	}
	return 0;
}