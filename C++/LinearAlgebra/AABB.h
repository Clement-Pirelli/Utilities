#ifndef AABB_H_DEFINED
#define AABB_H_DEFINED
#include <limits>
#include "vec.h"

#define _min(a,b) (a < b ? a : b)
#define _max(a,b) (a < b ? b : a)

template<uint32_t N>
class AABB
{
public:

	constexpr AABB(const vec<N> &givenMin, const vec<N> &givenMax) : min(givenMin), max(givenMax) {}
	constexpr AABB() {}

	static AABB<N> united(const AABB<N> &firstBox, const AABB<N> &secondBox)
	{
		vec2 min = {}, max = {};
		for (uint32_t i = 0; i < N; i++)
		{
			min[i] = _min(firstBox.min[i], secondBox.min[i]);
			max[i] = _max(firstBox.max[i], secondBox.max[i]);
		}
		return AABB<N>(min, max);
	}

	AABB<N> &boundInto(const AABB<N> &other)
	{
		min = min.clampedBy(other.min, other.max);
		max = max.clampedBy(other.min, other.max);
		return *this;
	}

	bool hasArea() const
	{
		for (uint32_t i = 0; i < N; i++)
		{
			if (isApproximatively(min[i], max[i], std::numeric_limits<float>::epsilon()))
				return false;
		}
		return true;
	}

	static inline float isApproximatively(float a, float b, float approx)
	{
		return (a + approx) > b && (a - approx) < b;
	}

	vec<N> min;
	vec<N> max;
};

using AABB2 = AABB<2>;
using AABB3 = AABB<3>;

#undef _min
#undef _max
#endif // !AABB_H_DEFINED