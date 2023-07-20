#include <vector>

template <class Ty>
class Stack
{
public:
	Stack();
	~Stack();

	void push(Ty val);
	Ty pop();
	Ty peek();
	void init();

private:
	std::vector<Ty> _stack;
};

template<class Ty>
Stack<Ty>::Stack() {

}

template<class Ty>
Stack<Ty>::~Stack() {

}

template<class Ty>
void Stack<Ty>::push(Ty val)
{
	_stack.push_back(val);
}

template<class Ty>
Ty Stack<Ty>::pop()
{
	if (_stack.empty()) throw std::exception("stack is empty");
	auto ret = _stack.back();
	_stack.pop_back();
	return ret;
}

template<class Ty>
Ty Stack<Ty>::peek()
{
	if (_stack.empty()) throw std::exception("stack is empty");
	Ty ret = _stack.back();
	return ret;
}

template<class Ty>
void Stack<Ty>::init()
{
	_stack.clear();
}
