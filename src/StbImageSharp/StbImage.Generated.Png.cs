// Generated by Sichem at 14.02.2020 1:27:28

using System;
using System.Runtime.InteropServices;

namespace StbImageSharp
{
	unsafe partial class StbImage
	{
		public const int STBI__F_none = 0;
		public const int STBI__F_sub = 1;
		public const int STBI__F_up = 2;
		public const int STBI__F_avg = 3;
		public const int STBI__F_paeth = 4;
		public const int STBI__F_avg_first = 5;
		public const int STBI__F_paeth_first = 6;

		public static byte[] png_sig = { 137, 80, 78, 71, 13, 10, 26, 10 };
		public static byte[] first_row_filter = { STBI__F_none, STBI__F_sub, STBI__F_none, STBI__F_avg_first, STBI__F_paeth_first };
		public static byte[] stbi__depth_scale_table = { 0, 0xff, 0x55, 0, 0x11, 0, 0, 0, 0x01 };

		[StructLayout(LayoutKind.Sequential)]
		public struct stbi__pngchunk
		{
			public uint length;
			public uint type;
		}

		public class stbi__png
		{
			public stbi__context s;
			public byte* idata;
			public byte* expanded;
			public byte* _out_;
			public int depth = 0;
		}

		public static stbi__pngchunk stbi__get_chunk_header(stbi__context s)
		{
			stbi__pngchunk c = new stbi__pngchunk();
			c.length = (uint)(stbi__get32be(s));
			c.type = (uint)(stbi__get32be(s));
			return (stbi__pngchunk)(c);
		}

		public static int stbi__check_png_header(stbi__context s)
		{
			int i = 0;
			for (i = (int)(0); (i) < (8); ++i)
			{
				if (stbi__get8(s) != png_sig[i])
					return (int)(stbi__err("bad png sig"));
			}
			return (int)(1);
		}

		public static int stbi__create_png_image_raw(stbi__png a, byte* raw, uint raw_len, int out_n, uint x, uint y, int depth, int color)
		{
			int bytes = (int)((depth) == (16) ? 2 : 1);
			stbi__context s = a.s;
			uint i = 0;
			uint j = 0;
			uint stride = (uint)(x * out_n * bytes);
			uint img_len = 0;
			uint img_width_bytes = 0;
			int k = 0;
			int img_n = (int)(s.img_n);
			int output_bytes = (int)(out_n * bytes);
			int filter_bytes = (int)(img_n * bytes);
			int width = (int)(x);
			a._out_ = (byte*)(stbi__malloc_mad3((int)(x), (int)(y), (int)(output_bytes), (int)(0)));
			if (a._out_ == null)
				return (int)(stbi__err("outofmem"));
			if (stbi__mad3sizes_valid((int)(img_n), (int)(x), (int)(depth), (int)(7)) == 0)
				return (int)(stbi__err("too large"));
			img_width_bytes = (uint)(((img_n * x * depth) + 7) >> 3);
			img_len = (uint)((img_width_bytes + 1) * y);
			if ((raw_len) < (img_len))
				return (int)(stbi__err("not enough pixels"));
			for (j = (uint)(0); (j) < (y); ++j)
			{
				byte* cur = a._out_ + stride * j;
				byte* prior;
				int filter = (int)(*raw++);
				if ((filter) > (4))
					return (int)(stbi__err("invalid filter"));
				if ((depth) < (8))
				{
					cur += x * out_n - img_width_bytes;
					filter_bytes = (int)(1);
					width = (int)(img_width_bytes);
				}
				prior = cur - stride;
				if ((j) == (0))
					filter = (int)(first_row_filter[filter]);
				for (k = (int)(0); (k) < (filter_bytes); ++k)
				{
					switch (filter)
					{
						case STBI__F_none:
							cur[k] = (byte)(raw[k]);
							break;
						case STBI__F_sub:
							cur[k] = (byte)(raw[k]);
							break;
						case STBI__F_up:
							cur[k] = ((byte)((raw[k] + prior[k]) & 255));
							break;
						case STBI__F_avg:
							cur[k] = ((byte)((raw[k] + (prior[k] >> 1)) & 255));
							break;
						case STBI__F_paeth:
							cur[k] = ((byte)((raw[k] + stbi__paeth((int)(0), (int)(prior[k]), (int)(0))) & 255));
							break;
						case STBI__F_avg_first:
							cur[k] = (byte)(raw[k]);
							break;
						case STBI__F_paeth_first:
							cur[k] = (byte)(raw[k]);
							break;
					}
				}
				if ((depth) == (8))
				{
					if (img_n != out_n)
						cur[img_n] = (byte)(255);
					raw += img_n;
					cur += out_n;
					prior += out_n;
				}
				else if ((depth) == (16))
				{
					if (img_n != out_n)
					{
						cur[filter_bytes] = (byte)(255);
						cur[filter_bytes + 1] = (byte)(255);
					}
					raw += filter_bytes;
					cur += output_bytes;
					prior += output_bytes;
				}
				else
				{
					raw += 1;
					cur += 1;
					prior += 1;
				}
				if (((depth) < (8)) || ((img_n) == (out_n)))
				{
					int nk = (int)((width - 1) * filter_bytes);
					switch (filter)
					{
						case STBI__F_none:
							CRuntime.memcpy(cur, raw, (ulong)(nk));
							break;
						case STBI__F_sub:
							for (k = (int)(0); (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + cur[k - filter_bytes]) & 255));
							}
							break;
						case STBI__F_up:
							for (k = (int)(0); (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + prior[k]) & 255));
							}
							break;
						case STBI__F_avg:
							for (k = (int)(0); (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + ((prior[k] + cur[k - filter_bytes]) >> 1)) & 255));
							}
							break;
						case STBI__F_paeth:
							for (k = (int)(0); (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + stbi__paeth((int)(cur[k - filter_bytes]), (int)(prior[k]), (int)(prior[k - filter_bytes]))) & 255));
							}
							break;
						case STBI__F_avg_first:
							for (k = (int)(0); (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + (cur[k - filter_bytes] >> 1)) & 255));
							}
							break;
						case STBI__F_paeth_first:
							for (k = (int)(0); (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + stbi__paeth((int)(cur[k - filter_bytes]), (int)(0), (int)(0))) & 255));
							}
							break;
					}
					raw += nk;
				}
				else
				{
					switch (filter)
					{
						case STBI__F_none:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = (byte)(raw[k]);
								}
							}
							break;
						case STBI__F_sub:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + cur[k - output_bytes]) & 255));
								}
							}
							break;
						case STBI__F_up:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + prior[k]) & 255));
								}
							}
							break;
						case STBI__F_avg:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + ((prior[k] + cur[k - output_bytes]) >> 1)) & 255));
								}
							}
							break;
						case STBI__F_paeth:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + stbi__paeth((int)(cur[k - output_bytes]), (int)(prior[k]), (int)(prior[k - output_bytes]))) & 255));
								}
							}
							break;
						case STBI__F_avg_first:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + (cur[k - output_bytes] >> 1)) & 255));
								}
							}
							break;
						case STBI__F_paeth_first:
							for (i = (uint)(x - 1); (i) >= (1); --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
							{
								for (k = (int)(0); (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + stbi__paeth((int)(cur[k - output_bytes]), (int)(0), (int)(0))) & 255));
								}
							}
							break;
					}
					if ((depth) == (16))
					{
						cur = a._out_ + stride * j;
						for (i = (uint)(0); (i) < (x); ++i, cur += output_bytes)
						{
							cur[filter_bytes + 1] = (byte)(255);
						}
					}
				}
			}
			if ((depth) < (8))
			{
				for (j = (uint)(0); (j) < (y); ++j)
				{
					byte* cur = a._out_ + stride * j;
					byte* _in_ = a._out_ + stride * j + x * out_n - img_width_bytes;
					byte scale = (byte)(((color) == (0)) ? stbi__depth_scale_table[depth] : 1);
					if ((depth) == (4))
					{
						for (k = (int)(x * img_n); (k) >= (2); k -= (int)(2), ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 4));
							*cur++ = (byte)(scale * ((*_in_) & 0x0f));
						}
						if ((k) > (0))
							*cur++ = (byte)(scale * (*_in_ >> 4));
					}
					else if ((depth) == (2))
					{
						for (k = (int)(x * img_n); (k) >= (4); k -= (int)(4), ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 6));
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
							*cur++ = (byte)(scale * ((*_in_) & 0x03));
						}
						if ((k) > (0))
							*cur++ = (byte)(scale * (*_in_ >> 6));
						if ((k) > (1))
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
						if ((k) > (2))
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
					}
					else if ((depth) == (1))
					{
						for (k = (int)(x * img_n); (k) >= (8); k -= (int)(8), ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 7));
							*cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
							*cur++ = (byte)(scale * ((*_in_) & 0x01));
						}
						if ((k) > (0))
							*cur++ = (byte)(scale * (*_in_ >> 7));
						if ((k) > (1))
							*cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
						if ((k) > (2))
							*cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
						if ((k) > (3))
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
						if ((k) > (4))
							*cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
						if ((k) > (5))
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
						if ((k) > (6))
							*cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
					}
					if (img_n != out_n)
					{
						int q = 0;
						cur = a._out_ + stride * j;
						if ((img_n) == (1))
						{
							for (q = (int)(x - 1); (q) >= (0); --q)
							{
								cur[q * 2 + 1] = (byte)(255);
								cur[q * 2 + 0] = (byte)(cur[q]);
							}
						}
						else
						{
							for (q = (int)(x - 1); (q) >= (0); --q)
							{
								cur[q * 4 + 3] = (byte)(255);
								cur[q * 4 + 2] = (byte)(cur[q * 3 + 2]);
								cur[q * 4 + 1] = (byte)(cur[q * 3 + 1]);
								cur[q * 4 + 0] = (byte)(cur[q * 3 + 0]);
							}
						}
					}
				}
			}
			else if ((depth) == (16))
			{
				byte* cur = a._out_;
				ushort* cur16 = (ushort*)(cur);
				for (i = (uint)(0); (i) < (x * y * out_n); ++i, cur16++, cur += 2)
				{
					*cur16 = (ushort)((cur[0] << 8) | cur[1]);
				}
			}

			return (int)(1);
		}

		public static int stbi__create_png_image(stbi__png a, byte* image_data, uint image_data_len, int out_n, int depth, int color, int interlaced)
		{
			int bytes = (int)((depth) == (16) ? 2 : 1);
			int out_bytes = (int)(out_n * bytes);
			byte* final;
			int p = 0;
			if (interlaced == 0)
				return (int)(stbi__create_png_image_raw(a, image_data, (uint)(image_data_len), (int)(out_n), (uint)(a.s.img_x), (uint)(a.s.img_y), (int)(depth), (int)(color)));
			final = (byte*)(stbi__malloc_mad3((int)(a.s.img_x), (int)(a.s.img_y), (int)(out_bytes), (int)(0)));
			for (p = (int)(0); (p) < (7); ++p)
			{
				int* xorig = stackalloc int[7];
				xorig[0] = (int)(0);
				xorig[1] = (int)(4);
				xorig[2] = (int)(0);
				xorig[3] = (int)(2);
				xorig[4] = (int)(0);
				xorig[5] = (int)(1);
				xorig[6] = (int)(0);
				int* yorig = stackalloc int[7];
				yorig[0] = (int)(0);
				yorig[1] = (int)(0);
				yorig[2] = (int)(4);
				yorig[3] = (int)(0);
				yorig[4] = (int)(2);
				yorig[5] = (int)(0);
				yorig[6] = (int)(1);
				int* xspc = stackalloc int[7];
				xspc[0] = (int)(8);
				xspc[1] = (int)(8);
				xspc[2] = (int)(4);
				xspc[3] = (int)(4);
				xspc[4] = (int)(2);
				xspc[5] = (int)(2);
				xspc[6] = (int)(1);
				int* yspc = stackalloc int[7];
				yspc[0] = (int)(8);
				yspc[1] = (int)(8);
				yspc[2] = (int)(8);
				yspc[3] = (int)(4);
				yspc[4] = (int)(4);
				yspc[5] = (int)(2);
				yspc[6] = (int)(2);
				int i = 0;
				int j = 0;
				int x = 0;
				int y = 0;
				x = (int)((a.s.img_x - xorig[p] + xspc[p] - 1) / xspc[p]);
				y = (int)((a.s.img_y - yorig[p] + yspc[p] - 1) / yspc[p]);
				if (((x) != 0) && ((y) != 0))
				{
					uint img_len = (uint)(((((a.s.img_n * x * depth) + 7) >> 3) + 1) * y);
					if (stbi__create_png_image_raw(a, image_data, (uint)(image_data_len), (int)(out_n), (uint)(x), (uint)(y), (int)(depth), (int)(color)) == 0)
					{
						CRuntime.free(final);
						return (int)(0);
					}
					for (j = (int)(0); (j) < (y); ++j)
					{
						for (i = (int)(0); (i) < (x); ++i)
						{
							int out_y = (int)(j * yspc[p] + yorig[p]);
							int out_x = (int)(i * xspc[p] + xorig[p]);
							CRuntime.memcpy(final + out_y * a.s.img_x * out_bytes + out_x * out_bytes, a._out_ + (j * x + i) * out_bytes, (ulong)(out_bytes));
						}
					}
					CRuntime.free(a._out_);
					image_data += img_len;
					image_data_len -= (uint)(img_len);
				}
			}
			a._out_ = final;
			return (int)(1);
		}

		public static int stbi__compute_transparency(stbi__png z, byte* tc, int out_n)
		{
			stbi__context s = z.s;
			uint i = 0;
			uint pixel_count = (uint)(s.img_x * s.img_y);
			byte* p = z._out_;
			if ((out_n) == (2))
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					p[1] = (byte)((p[0]) == (tc[0]) ? 0 : 255);
					p += 2;
				}
			}
			else
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					if ((((p[0]) == (tc[0])) && ((p[1]) == (tc[1]))) && ((p[2]) == (tc[2])))
						p[3] = (byte)(0);
					p += 4;
				}
			}

			return (int)(1);
		}

		public static int stbi__compute_transparency16(stbi__png z, ushort* tc, int out_n)
		{
			stbi__context s = z.s;
			uint i = 0;
			uint pixel_count = (uint)(s.img_x * s.img_y);
			ushort* p = (ushort*)(z._out_);
			if ((out_n) == (2))
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					p[1] = (ushort)((p[0]) == (tc[0]) ? 0 : 65535);
					p += 2;
				}
			}
			else
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					if ((((p[0]) == (tc[0])) && ((p[1]) == (tc[1]))) && ((p[2]) == (tc[2])))
						p[3] = (ushort)(0);
					p += 4;
				}
			}

			return (int)(1);
		}

		public static int stbi__expand_png_palette(stbi__png a, byte* palette, int len, int pal_img_n)
		{
			uint i = 0;
			uint pixel_count = (uint)(a.s.img_x * a.s.img_y);
			byte* p;
			byte* temp_out;
			byte* orig = a._out_;
			p = (byte*)(stbi__malloc_mad2((int)(pixel_count), (int)(pal_img_n), (int)(0)));
			if ((p) == (null))
				return (int)(stbi__err("outofmem"));
			temp_out = p;
			if ((pal_img_n) == (3))
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					int n = (int)(orig[i] * 4);
					p[0] = (byte)(palette[n]);
					p[1] = (byte)(palette[n + 1]);
					p[2] = (byte)(palette[n + 2]);
					p += 3;
				}
			}
			else
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					int n = (int)(orig[i] * 4);
					p[0] = (byte)(palette[n]);
					p[1] = (byte)(palette[n + 1]);
					p[2] = (byte)(palette[n + 2]);
					p[3] = (byte)(palette[n + 3]);
					p += 4;
				}
			}

			CRuntime.free(a._out_);
			a._out_ = temp_out;
			return (int)(1);
		}

		public static void stbi__de_iphone(stbi__png z)
		{
			stbi__context s = z.s;
			uint i = 0;
			uint pixel_count = (uint)(s.img_x * s.img_y);
			byte* p = z._out_;
			if ((s.img_out_n) == (3))
			{
				for (i = (uint)(0); (i) < (pixel_count); ++i)
				{
					byte t = (byte)(p[0]);
					p[0] = (byte)(p[2]);
					p[2] = (byte)(t);
					p += 3;
				}
			}
			else
			{
				if ((stbi__unpremultiply_on_load) != 0)
				{
					for (i = (uint)(0); (i) < (pixel_count); ++i)
					{
						byte a = (byte)(p[3]);
						byte t = (byte)(p[0]);
						if ((a) != 0)
						{
							byte half = (byte)(a / 2);
							p[0] = (byte)((p[2] * 255 + half) / a);
							p[1] = (byte)((p[1] * 255 + half) / a);
							p[2] = (byte)((t * 255 + half) / a);
						}
						else
						{
							p[0] = (byte)(p[2]);
							p[2] = (byte)(t);
						}
						p += 4;
					}
				}
				else
				{
					for (i = (uint)(0); (i) < (pixel_count); ++i)
					{
						byte t = (byte)(p[0]);
						p[0] = (byte)(p[2]);
						p[2] = (byte)(t);
						p += 4;
					}
				}
			}

		}

		public static int stbi__parse_png_file(stbi__png z, int scan, int req_comp)
		{
			byte* palette = stackalloc byte[1024];
			byte pal_img_n = (byte)(0);
			byte has_trans = (byte)(0);
			byte* tc = stackalloc byte[3];
			tc[0] = (byte)(0);

			ushort* tc16 = stackalloc ushort[3];
			uint ioff = (uint)(0);
			uint idata_limit = (uint)(0);
			uint i = 0;
			uint pal_len = (uint)(0);
			int first = (int)(1);
			int k = 0;
			int interlace = (int)(0);
			int color = (int)(0);
			int is_iphone = (int)(0);
			stbi__context s = z.s;
			z.expanded = (null);
			z.idata = (null);
			z._out_ = (null);
			if (stbi__check_png_header(s) == 0)
				return (int)(0);
			if ((scan) == (STBI__SCAN_type))
				return (int)(1);
			for (; ; )
			{
				stbi__pngchunk c = (stbi__pngchunk)(stbi__get_chunk_header(s));
				switch (c.type)
				{
					case (((uint)('C') << 24) + ((uint)('g') << 16) + ((uint)('B') << 8) + (uint)('I')):
						is_iphone = (int)(1);
						stbi__skip(s, (int)(c.length));
						break;
					case (((uint)('I') << 24) + ((uint)('H') << 16) + ((uint)('D') << 8) + (uint)('R')):
					{
						int comp = 0;
						int filter = 0;
						if (first == 0)
							return (int)(stbi__err("multiple IHDR"));
						first = (int)(0);
						if (c.length != 13)
							return (int)(stbi__err("bad IHDR len"));
						s.img_x = (uint)(stbi__get32be(s));
						if ((s.img_x) > (1 << 24))
							return (int)(stbi__err("too large"));
						s.img_y = (uint)(stbi__get32be(s));
						if ((s.img_y) > (1 << 24))
							return (int)(stbi__err("too large"));
						z.depth = (int)(stbi__get8(s));
						if (((((z.depth != 1) && (z.depth != 2)) && (z.depth != 4)) && (z.depth != 8)) && (z.depth != 16))
							return (int)(stbi__err("1/2/4/8/16-bit only"));
						color = (int)(stbi__get8(s));
						if ((color) > (6))
							return (int)(stbi__err("bad ctype"));
						if (((color) == (3)) && ((z.depth) == (16)))
							return (int)(stbi__err("bad ctype"));
						if ((color) == (3))
							pal_img_n = (byte)(3);
						else if ((color & 1) != 0)
							return (int)(stbi__err("bad ctype"));
						comp = (int)(stbi__get8(s));
						if ((comp) != 0)
							return (int)(stbi__err("bad comp method"));
						filter = (int)(stbi__get8(s));
						if ((filter) != 0)
							return (int)(stbi__err("bad filter method"));
						interlace = (int)(stbi__get8(s));
						if ((interlace) > (1))
							return (int)(stbi__err("bad interlace method"));
						if ((s.img_x == 0) || (s.img_y == 0))
							return (int)(stbi__err("0-pixel image"));
						if (pal_img_n == 0)
						{
							s.img_n = (int)(((color & 2) != 0 ? 3 : 1) + ((color & 4) != 0 ? 1 : 0));
							if (((1 << 30) / s.img_x / s.img_n) < (s.img_y))
								return (int)(stbi__err("too large"));
							if ((scan) == (STBI__SCAN_header))
								return (int)(1);
						}
						else
						{
							s.img_n = (int)(1);
							if (((1 << 30) / s.img_x / 4) < (s.img_y))
								return (int)(stbi__err("too large"));
						}
						break;
					}
					case (((uint)('P') << 24) + ((uint)('L') << 16) + ((uint)('T') << 8) + (uint)('E')):
					{
						if ((first) != 0)
							return (int)(stbi__err("first not IHDR"));
						if ((c.length) > (256 * 3))
							return (int)(stbi__err("invalid PLTE"));
						pal_len = (uint)(c.length / 3);
						if (pal_len * 3 != c.length)
							return (int)(stbi__err("invalid PLTE"));
						for (i = (uint)(0); (i) < (pal_len); ++i)
						{
							palette[i * 4 + 0] = (byte)(stbi__get8(s));
							palette[i * 4 + 1] = (byte)(stbi__get8(s));
							palette[i * 4 + 2] = (byte)(stbi__get8(s));
							palette[i * 4 + 3] = (byte)(255);
						}
						break;
					}
					case (((uint)('t') << 24) + ((uint)('R') << 16) + ((uint)('N') << 8) + (uint)('S')):
					{
						if ((first) != 0)
							return (int)(stbi__err("first not IHDR"));
						if ((z.idata) != null)
							return (int)(stbi__err("tRNS after IDAT"));
						if ((pal_img_n) != 0)
						{
							if ((scan) == (STBI__SCAN_header))
							{
								s.img_n = (int)(4);
								return (int)(1);
							}
							if ((pal_len) == (0))
								return (int)(stbi__err("tRNS before PLTE"));
							if ((c.length) > (pal_len))
								return (int)(stbi__err("bad tRNS len"));
							pal_img_n = (byte)(4);
							for (i = (uint)(0); (i) < (c.length); ++i)
							{
								palette[i * 4 + 3] = (byte)(stbi__get8(s));
							}
						}
						else
						{
							if ((s.img_n & 1) == 0)
								return (int)(stbi__err("tRNS with alpha"));
							if (c.length != (uint)(s.img_n) * 2)
								return (int)(stbi__err("bad tRNS len"));
							has_trans = (byte)(1);
							if ((z.depth) == (16))
							{
								for (k = (int)(0); (k) < (s.img_n); ++k)
								{
									tc16[k] = ((ushort)(stbi__get16be(s)));
								}
							}
							else
							{
								for (k = (int)(0); (k) < (s.img_n); ++k)
								{
									tc[k] = (byte)((byte)(stbi__get16be(s) & 255) * stbi__depth_scale_table[z.depth]);
								}
							}
						}
						break;
					}
					case (((uint)('I') << 24) + ((uint)('D') << 16) + ((uint)('A') << 8) + (uint)('T')):
					{
						if ((first) != 0)
							return (int)(stbi__err("first not IHDR"));
						if (((pal_img_n) != 0) && (pal_len == 0))
							return (int)(stbi__err("no PLTE"));
						if ((scan) == (STBI__SCAN_header))
						{
							s.img_n = (int)(pal_img_n);
							return (int)(1);
						}
						if (((int)(ioff + c.length)) < ((int)(ioff)))
							return (int)(0);
						if ((ioff + c.length) > (idata_limit))
						{
							uint idata_limit_old = (uint)(idata_limit);
							byte* p;
							if ((idata_limit) == (0))
								idata_limit = (uint)((c.length) > (4096) ? c.length : 4096);
							while ((ioff + c.length) > (idata_limit))
							{
								idata_limit *= (uint)(2);
							}
							p = (byte*)(CRuntime.realloc(z.idata, (ulong)(idata_limit)));
							if ((p) == (null))
								return (int)(stbi__err("outofmem"));
							z.idata = p;
						}
						if (stbi__getn(s, z.idata + ioff, (int)(c.length)) == 0)
							return (int)(stbi__err("outofdata"));
						ioff += (uint)(c.length);
						break;
					}
					case (((uint)('I') << 24) + ((uint)('E') << 16) + ((uint)('N') << 8) + (uint)('D')):
					{
						uint raw_len = 0;
						uint bpl = 0;
						if ((first) != 0)
							return (int)(stbi__err("first not IHDR"));
						if (scan != STBI__SCAN_load)
							return (int)(1);
						if ((z.idata) == (null))
							return (int)(stbi__err("no IDAT"));
						bpl = (uint)((s.img_x * z.depth + 7) / 8);
						raw_len = (uint)(bpl * s.img_y * s.img_n + s.img_y);
						z.expanded = (byte*)(stbi_zlib_decode_malloc_guesssize_headerflag((sbyte*)(z.idata), (int)(ioff), (int)(raw_len), (int*)(&raw_len), is_iphone != 0 ? 0 : 1));
						if ((z.expanded) == (null))
							return (int)(0);
						CRuntime.free(z.idata);
						z.idata = (null);
						if (((((req_comp) == (s.img_n + 1)) && (req_comp != 3)) && (pal_img_n == 0)) || ((has_trans) != 0))
							s.img_out_n = (int)(s.img_n + 1);
						else
							s.img_out_n = (int)(s.img_n);
						if (stbi__create_png_image(z, z.expanded, (uint)(raw_len), (int)(s.img_out_n), (int)(z.depth), (int)(color), (int)(interlace)) == 0)
							return (int)(0);
						if ((has_trans) != 0)
						{
							if ((z.depth) == (16))
							{
								if (stbi__compute_transparency16(z, tc16, (int)(s.img_out_n)) == 0)
									return (int)(0);
							}
							else
							{
								if (stbi__compute_transparency(z, tc, (int)(s.img_out_n)) == 0)
									return (int)(0);
							}
						}
						if ((((is_iphone) != 0) && ((stbi__de_iphone_flag) != 0)) && ((s.img_out_n) > (2)))
							stbi__de_iphone(z);
						if ((pal_img_n) != 0)
						{
							s.img_n = (int)(pal_img_n);
							s.img_out_n = (int)(pal_img_n);
							if ((req_comp) >= (3))
								s.img_out_n = (int)(req_comp);
							if (stbi__expand_png_palette(z, palette, (int)(pal_len), (int)(s.img_out_n)) == 0)
								return (int)(0);
						}
						else if ((has_trans) != 0)
						{
							++s.img_n;
						}
						CRuntime.free(z.expanded);
						z.expanded = (null);
						return (int)(1);
					}
					default:
						if ((first) != 0)
							return (int)(stbi__err("first not IHDR"));
						if ((c.type & (1 << 29)) == (0))
						{
							string invalid_chunk = c.type + " PNG chunk not known";
							return (int)(stbi__err(invalid_chunk));
						}
						stbi__skip(s, (int)(c.length));
						break;
				}
				stbi__get32be(s);
			}
		}

		public static void* stbi__do_png(stbi__png p, int* x, int* y, int* n, int req_comp, stbi__result_info* ri)
		{
			void* result = (null);
			if (((req_comp) < (0)) || ((req_comp) > (4)))
				return ((byte*)((ulong)((stbi__err("bad req_comp")) != 0 ? ((byte*)null) : (null))));
			if ((stbi__parse_png_file(p, (int)(STBI__SCAN_load), (int)(req_comp))) != 0)
			{
				if ((p.depth) < (8))
					ri->bits_per_channel = (int)(8);
				else
					ri->bits_per_channel = (int)(p.depth);
				result = p._out_;
				p._out_ = (null);
				if (((req_comp) != 0) && (req_comp != p.s.img_out_n))
				{
					if ((ri->bits_per_channel) == (8))
						result = stbi__convert_format((byte*)(result), (int)(p.s.img_out_n), (int)(req_comp), (uint)(p.s.img_x), (uint)(p.s.img_y));
					else
						result = stbi__convert_format16((ushort*)(result), (int)(p.s.img_out_n), (int)(req_comp), (uint)(p.s.img_x), (uint)(p.s.img_y));
					p.s.img_out_n = (int)(req_comp);
					if ((result) == (null))
						return result;
				}
				*x = (int)(p.s.img_x);
				*y = (int)(p.s.img_y);
				if ((n) != null)
					*n = (int)(p.s.img_n);
			}

			CRuntime.free(p._out_);
			p._out_ = (null);
			CRuntime.free(p.expanded);
			p.expanded = (null);
			CRuntime.free(p.idata);
			p.idata = (null);
			return result;
		}

		public static void* stbi__png_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri)
		{
			stbi__png p = new stbi__png();
			p.s = s;
			return stbi__do_png(p, x, y, comp, (int)(req_comp), ri);
		}

		public static int stbi__png_test(stbi__context s)
		{
			int r = 0;
			r = (int)(stbi__check_png_header(s));
			stbi__rewind(s);
			return (int)(r);
		}

		public static int stbi__png_info_raw(stbi__png p, int* x, int* y, int* comp)
		{
			if (stbi__parse_png_file(p, (int)(STBI__SCAN_header), (int)(0)) == 0)
			{
				stbi__rewind(p.s);
				return (int)(0);
			}

			if ((x) != null)
				*x = (int)(p.s.img_x);
			if ((y) != null)
				*y = (int)(p.s.img_y);
			if ((comp) != null)
				*comp = (int)(p.s.img_n);
			return (int)(1);
		}

		public static int stbi__png_info(stbi__context s, int* x, int* y, int* comp)
		{
			stbi__png p = new stbi__png();
			p.s = s;
			return (int)(stbi__png_info_raw(p, x, y, comp));
		}

		public static int stbi__png_is16(stbi__context s)
		{
			stbi__png p = new stbi__png();
			p.s = s;
			if (stbi__png_info_raw(p, (null), (null), (null)) == 0)
				return (int)(0);
			if (p.depth != 16)
			{
				stbi__rewind(p.s);
				return (int)(0);
			}

			return (int)(1);
		}
	}
}
