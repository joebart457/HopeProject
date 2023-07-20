#include <map>
#include <any>

template <class Ty>
class Slice
{
public:
	Slice();
	~Slice();

	std::any get(Ty key);
	void put(Ty key, std::any& val);
	bool exists(Ty key);
private:
	std::map<Ty, std::any> _lookup;
};


#include <sstream>

template<class Ty>
inline Slice<Ty>::Slice()
{
}

template<class Ty>
inline Slice<Ty>::~Slice()
{
}

template<class Ty>
inline std::any Slice<Ty>::get(Ty key)
{
	if (_lookup.count(key) == 0) {
		std::ostringstream oss;
		oss << "Key " << key << " not found in stor";
		throw std::exception(oss.str().c_str());
	}
	return _lookup[key];
}

template<class Ty>
inline void Slice<Ty>::put(Ty key, std::any& val)
{
	_lookup[key] = val;
}

template<class Ty>
inline bool Slice<Ty>::exists(Ty key)
{
	return _lookup.count(key);
}
