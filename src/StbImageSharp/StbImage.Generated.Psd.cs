// Generated by Sichem at 14.02.2020 1:27:28

using System;
using System.Runtime.InteropServices;

namespace StbImageSharp
{
	unsafe partial class StbImage
	{
		public static int stbi__psd_test(stbi__context s)
		{
			int r = (((stbi__get32be(s)) == (0x38425053))) ? 1 : 0;
			stbi__rewind(s);
			return (int)(r);
		}

		public static int stbi__psd_decode_rle(stbi__context s, byte* p, int pixelCount)
		{
			int count = 0;
			int nleft = 0;
			int len = 0;
			count = (int)(0);
			while ((nleft = (int)(pixelCount - count)) > (0))
			{
				len = (int)(stbi__get8(s));
				if ((len) == (128))
				{
				}
				else if ((len) < (128))
				{
					len++;
					if ((len) > (nleft))
						return (int)(0);
					count += (int)(len);
					while ((len) != 0)
					{
						*p = (byte)(stbi__get8(s));
						p += 4;
						len--;
					}
				}
				else if ((len) > (128))
				{
					byte val = 0;
					len = (int)(257 - len);
					if ((len) > (nleft))
						return (int)(0);
					val = (byte)(stbi__get8(s));
					count += (int)(len);
					while ((len) != 0)
					{
						*p = (byte)(val);
						p += 4;
						len--;
					}
				}
			}
			return (int)(1);
		}

		public static void* stbi__psd_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri, int bpc)
		{
			int pixelCount = 0;
			int channelCount = 0;
			int compression = 0;
			int channel = 0;
			int i = 0;
			int bitdepth = 0;
			int w = 0;
			int h = 0;
			byte* _out_;
			if (stbi__get32be(s) != 0x38425053)
				return ((byte*)((ulong)((stbi__err("not PSD")) != 0 ? ((byte*)null) : (null))));
			if (stbi__get16be(s) != 1)
				return ((byte*)((ulong)((stbi__err("wrong version")) != 0 ? ((byte*)null) : (null))));
			stbi__skip(s, (int)(6));
			channelCount = (int)(stbi__get16be(s));
			if (((channelCount) < (0)) || ((channelCount) > (16)))
				return ((byte*)((ulong)((stbi__err("wrong channel count")) != 0 ? ((byte*)null) : (null))));
			h = (int)(stbi__get32be(s));
			w = (int)(stbi__get32be(s));
			bitdepth = (int)(stbi__get16be(s));
			if ((bitdepth != 8) && (bitdepth != 16))
				return ((byte*)((ulong)((stbi__err("unsupported bit depth")) != 0 ? ((byte*)null) : (null))));
			if (stbi__get16be(s) != 3)
				return ((byte*)((ulong)((stbi__err("wrong color format")) != 0 ? ((byte*)null) : (null))));
			stbi__skip(s, (int)(stbi__get32be(s)));
			stbi__skip(s, (int)(stbi__get32be(s)));
			stbi__skip(s, (int)(stbi__get32be(s)));
			compression = (int)(stbi__get16be(s));
			if ((compression) > (1))
				return ((byte*)((ulong)((stbi__err("bad compression")) != 0 ? ((byte*)null) : (null))));
			if (stbi__mad3sizes_valid((int)(4), (int)(w), (int)(h), (int)(0)) == 0)
				return ((byte*)((ulong)((stbi__err("too large")) != 0 ? ((byte*)null) : (null))));
			if (((compression == 0) && ((bitdepth) == (16))) && ((bpc) == (16)))
			{
				_out_ = (byte*)(stbi__malloc_mad3((int)(8), (int)(w), (int)(h), (int)(0)));
				ri->bits_per_channel = (int)(16);
			}
			else
				_out_ = (byte*)(stbi__malloc((ulong)(4 * w * h)));
			if (_out_ == null)
				return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
			pixelCount = (int)(w * h);
			if ((compression) != 0)
			{
				stbi__skip(s, (int)(h * channelCount * 2));
				for (channel = (int)(0); (channel) < (4); channel++)
				{
					byte* p;
					p = _out_ + channel;
					if ((channel) >= (channelCount))
					{
						for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
						{
							*p = (byte)((channel) == (3) ? 255 : 0);
						}
					}
					else
					{
						if (stbi__psd_decode_rle(s, p, (int)(pixelCount)) == 0)
						{
							CRuntime.free(_out_);
							return ((byte*)((ulong)((stbi__err("corrupt")) != 0 ? ((byte*)null) : (null))));
						}
					}
				}
			}
			else
			{
				for (channel = (int)(0); (channel) < (4); channel++)
				{
					if ((channel) >= (channelCount))
					{
						if (((bitdepth) == (16)) && ((bpc) == (16)))
						{
							ushort* q = ((ushort*)(_out_)) + channel;
							ushort val = (ushort)((channel) == (3) ? 65535 : 0);
							for (i = (int)(0); (i) < (pixelCount); i++, q += 4)
							{
								*q = (ushort)(val);
							}
						}
						else
						{
							byte* p = _out_ + channel;
							byte val = (byte)((channel) == (3) ? 255 : 0);
							for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
							{
								*p = (byte)(val);
							}
						}
					}
					else
					{
						if ((ri->bits_per_channel) == (16))
						{
							ushort* q = ((ushort*)(_out_)) + channel;
							for (i = (int)(0); (i) < (pixelCount); i++, q += 4)
							{
								*q = ((ushort)(stbi__get16be(s)));
							}
						}
						else
						{
							byte* p = _out_ + channel;
							if ((bitdepth) == (16))
							{
								for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
								{
									*p = ((byte)(stbi__get16be(s) >> 8));
								}
							}
							else
							{
								for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
								{
									*p = (byte)(stbi__get8(s));
								}
							}
						}
					}
				}
			}

			if ((channelCount) >= (4))
			{
				if ((ri->bits_per_channel) == (16))
				{
					for (i = (int)(0); (i) < (w * h); ++i)
					{
						ushort* pixel = (ushort*)(_out_) + 4 * i;
						if ((pixel[3] != 0) && (pixel[3] != 65535))
						{
							float a = (float)(pixel[3] / 65535.0f);
							float ra = (float)(1.0f / a);
							float inv_a = (float)(65535.0f * (1 - ra));
							pixel[0] = ((ushort)(pixel[0] * ra + inv_a));
							pixel[1] = ((ushort)(pixel[1] * ra + inv_a));
							pixel[2] = ((ushort)(pixel[2] * ra + inv_a));
						}
					}
				}
				else
				{
					for (i = (int)(0); (i) < (w * h); ++i)
					{
						byte* pixel = _out_ + 4 * i;
						if ((pixel[3] != 0) && (pixel[3] != 255))
						{
							float a = (float)(pixel[3] / 255.0f);
							float ra = (float)(1.0f / a);
							float inv_a = (float)(255.0f * (1 - ra));
							pixel[0] = ((byte)(pixel[0] * ra + inv_a));
							pixel[1] = ((byte)(pixel[1] * ra + inv_a));
							pixel[2] = ((byte)(pixel[2] * ra + inv_a));
						}
					}
				}
			}

			if (((req_comp) != 0) && (req_comp != 4))
			{
				if ((ri->bits_per_channel) == (16))
					_out_ = (byte*)(stbi__convert_format16((ushort*)(_out_), (int)(4), (int)(req_comp), (uint)(w), (uint)(h)));
				else
					_out_ = stbi__convert_format(_out_, (int)(4), (int)(req_comp), (uint)(w), (uint)(h));
				if ((_out_) == (null))
					return _out_;
			}

			if ((comp) != null)
				*comp = (int)(4);
			*y = (int)(h);
			*x = (int)(w);
			return _out_;
		}

		public static int stbi__psd_info(stbi__context s, int* x, int* y, int* comp)
		{
			int channelCount = 0;
			int dummy = 0;
			int depth = 0;
			if (x == null)
				x = &dummy;
			if (y == null)
				y = &dummy;
			if (comp == null)
				comp = &dummy;
			if (stbi__get32be(s) != 0x38425053)
			{
				stbi__rewind(s);
				return (int)(0);
			}

			if (stbi__get16be(s) != 1)
			{
				stbi__rewind(s);
				return (int)(0);
			}

			stbi__skip(s, (int)(6));
			channelCount = (int)(stbi__get16be(s));
			if (((channelCount) < (0)) || ((channelCount) > (16)))
			{
				stbi__rewind(s);
				return (int)(0);
			}

			*y = (int)(stbi__get32be(s));
			*x = (int)(stbi__get32be(s));
			depth = (int)(stbi__get16be(s));
			if ((depth != 8) && (depth != 16))
			{
				stbi__rewind(s);
				return (int)(0);
			}

			if (stbi__get16be(s) != 3)
			{
				stbi__rewind(s);
				return (int)(0);
			}

			*comp = (int)(4);
			return (int)(1);
		}

		public static int stbi__psd_is16(stbi__context s)
		{
			int channelCount = 0;
			int depth = 0;
			if (stbi__get32be(s) != 0x38425053)
			{
				stbi__rewind(s);
				return (int)(0);
			}

			if (stbi__get16be(s) != 1)
			{
				stbi__rewind(s);
				return (int)(0);
			}

			stbi__skip(s, (int)(6));
			channelCount = (int)(stbi__get16be(s));
			if (((channelCount) < (0)) || ((channelCount) > (16)))
			{
				stbi__rewind(s);
				return (int)(0);
			}

			stbi__get32be(s);
			stbi__get32be(s);
			depth = (int)(stbi__get16be(s));
			if (depth != 16)
			{
				stbi__rewind(s);
				return (int)(0);
			}

			return (int)(1);
		}

	}
}