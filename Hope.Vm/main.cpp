
#include "vm.hpp"
#include "ContextBuilder.hpp"
#include <conio.h>
#include <fstream>

int main(int argc, char** argv) {

	vm machine(ContextBuilder().create_std_lib());


	std::ifstream file("C:\\Users\\Jimmy Barnes\\Documents\\HopeProject\\Hope.Compiler\\bin\\Debug\\net6.0\\out", std::ios::binary | std::ios::ate);
	std::streamsize size = file.tellg();
	file.seekg(0, std::ios::beg);

	std::vector<char> buffer(size);
	if (file.read(buffer.data(), size))
	{
		return machine.run(buffer.data(), size);
	}
	else {
		std::cout << "cannot open file";
	}

	_getch();
	return 0;
}