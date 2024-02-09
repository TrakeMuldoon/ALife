using System;
using System.Collections.Generic;

namespace LGridNamespace
{
    public struct LGrid
    {
        public FreeList<LGridElt> elts;
        public LGridLoose loose;
        public int num_elts;
        public LGridTight tight;
        public float w, h;
        public float x, y;
    }

    public struct LGridElt
    {
        public float hx, hy;
        public int id;
        public float mx, my;
        public int next;
    }

    public struct LGridLoose
    {
        public LGridLooseCell[] cells;
        public float inv_cell_w, inv_cell_h;
        public int num_cols, num_rows, num_cells;
    }

    public struct LGridLooseCell
    {
        public int head;
        public float[] rect;
    }

    public struct LGridQuery4
    {
        public SmallList<int>[] elements;
    }

    public struct LGridTight
    {
        public FreeList<LGridTightCell> cells;
        public int[] heads;
        public float inv_cell_w, inv_cell_h;
        public int num_cols, num_rows, num_cells;
    }

    public struct LGridTightCell
    {
        public int lcell;
        public int next;
    }

    public class FreeList<T>
    {
        private SmallList<FreeElement> data;

        private int first_free;

        public FreeList()
        {
            first_free = -1;
        }

        public T this[int n]
        {
            get
            {
                if(n >= 0 && n < data.size())
                    return data[n].element;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if(n >= 0 && n < data.size())
                    data[n].element = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public void clear()
        {
            data.clear();
            first_free = -1;
        }

        public void erase(int n)
        {
            if(n >= 0 && n < data.size())
            {
                data[n].next = first_free;
                first_free = n;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public int insert(T element)
        {
            if(first_free != -1)
            {
                int index = first_free;
                first_free = data[first_free].next;
                data[index].element = element;
                return index;
            }
            else
            {
                FreeElement fe = new FreeElement();
                fe.element = element;
                data.push_back(fe);
                return data.size() - 1;
            }
        }

        public int range()
        {
            return data.size();
        }

        public void reserve(int n)
        {
            data.reserve(n);
        }

        public void swap(FreeList<T> other)
        {
            int temp = first_free;
            data.swap(other.data);
            first_free = other.first_free;
            other.first_free = temp;
        }

        private struct FreeElement
        {
            public T element;
            public int next;
        }
    }

    public class LGridClass
    {
        LGrid grid = new LGrid();

        int num_lcols = ceil_div(w, lcell_w), num_lrows = ceil_div(h, lcell_h);

        int num_tcols = ceil_div(w, tcell_w), num_trows = ceil_div(h, tcell_h);

        float w = r - l, h = b - t;

        public static LGrid lgrid_create(float lcell_w, float lcell_h, float tcell_w, float tcell_h,
        float l, float t, float r, float b)a

        {
        grid.num_elts = 0;
        grid.x = l;
        grid.y = t;
        grid.h = w;
        grid.w = h;
        grid.loose.num_cols = num_lcols;
        grid.loose.num_rows = num_lrows;

        grid.loose.num_cells = grid.loose.num_cols* grid.loose.num_rows;

        grid.loose.inv_cell_w = 1.0f / lcell_w;
        grid.loose.inv_cell_h = 1.0f / lcell_h;
        grid.tight.num_cols = num_tcols;
        grid.tight.num_rows = num_trows;

        grid.tight.num_cells = grid.tight.num_cols* grid.tight.num_rows;

        grid.tight.inv_cell_w = 1.0f / tcell_w;
        grid.tight.inv_cell_h = 1.0f / tcell_h;

        grid.tight.heads = new int[grid.tight.num_cells];
            for(int j = 0; j<grid.tight.num_cells; ++j)
        grid.tight.heads[j] = -1;

        grid.loose.cells = new LGridLooseCell[grid.loose.num_cells];
            for(int c = 0; c<grid.loose.num_cells; ++c)
            {
        grid.loose.cells[c].head = -1;
        grid.loose.cells[c].rect = new float[4] { float.MaxValue, float.MaxValue, -float.MaxValue, -float.MaxValue
    };
}

return grid;
        }

        public static void lgrid_destroy(LGrid grid)
{
    Array.Clear(grid.loose.cells, 0, grid.loose.cells.Length);
    Array.Clear(grid.tight.heads, 0, grid.tight.heads.Length);
}

public static void lgrid_insert(LGrid grid, int id, float mx, float my, float hx, float hy)
{
    int cell_idx = lgrid_lcell_idx(grid, mx, my);
    LGridLooseCell lcell = grid.loose.cells[cell_idx];

    LGridElt new_elt = new LGridElt { next = lcell.head, id = id, mx = mx - grid.x, my = my - grid.y, hx = hx, hy = hy };
    lcell.head = grid.elts.insert(new_elt);
    ++grid.num_elts;

    expand_aabb(grid, cell_idx, mx, my, hx, hy);
}

public static int lgrid_lcell_idx(LGrid grid, float x, float y)
{
    int cell_x = to_cell_idx(x - grid.x, grid.loose.inv_cell_w, grid.loose.num_cols);
    int cell_y = to_cell_idx(y - grid.y, grid.loose.inv_cell_h, grid.loose.num_rows);
    return cell_y * grid.loose.num_cols + cell_x;
}

private static int ceil_div(float value, float divisor)
{
    float resultf = value / divisor;
    int result = (int)resultf;
    return result < resultf ? result + 1 : result;
}

private static __m128 element_rect(LGridElt elt)
{
    return simd_create4f(elt.mx - elt.hx, elt.my - elt.hy,
                         elt.mx + elt.hx, elt.my + elt.hy);
}

private static void expand_aabb(LGrid grid, int cell_idx, float mx, float my, float hx, float hy)
{
    LGridLooseCell lcell = grid.loose.cells[cell_idx];
    float[] prev_rect = new float[4] { lcell.rect[0], lcell.rect[1], lcell.rect[2], lcell.rect[3] };
    lcell.rect[0] = min_flt(lcell.rect[0], mx - hx);
    lcell.rect[1] = min_flt(lcell.rect[1], my - hy);
    lcell.rect[2] = max_flt(lcell.rect[2], mx + hx);
    lcell.rect[3] = max_flt(lcell.rect[3], my + hy);

    float[] elt_rect = new float[4] { mx - hx, my - hy, mx + hx, my + hy };
    SimdVec4i trect = to_tcell_idx4(grid, simd_load4f(elt_rect));
    if(prev_rect[0] > prev_rect[2])
    {
        for(int ty = trect.data[1]; ty <= trect.data[3]; ++ty)
        {
            int[] tight_row = grid.tight.heads + ty * grid.tight.num_cols;
            for(int tx = trect.data[0]; tx <= trect.data[2]; ++tx)
            {
                LGridTightCell new_tcell = new LGridTightCell { next = tight_row[tx], lcell = cell_idx };
                tight_row[tx] = grid.tight.cells.insert(new_tcell);
            }
        }
    }
    else
    {
        SimdVec4i prev_trect = to_tcell_idx4(grid, simd_load4f(prev_rect));
        if(trect.data[0] != prev_trect.data[0] || trect.data[1] != prev_trect.data[1] ||
            trect.data[2] != prev_trect.data[2] || trect.data[3] != prev_trect.data[3])
        {
            for(int ty = trect.data[1]; ty <= trect.data[3]; ++ty)
            {
                int[] tight_row = grid.tight.heads + ty * grid.tight.num_cols;
                for(int tx = trect.data[0]; tx <= trect.data[2]; ++tx)
                {
                    if(tx < prev_trect.data[0] || tx > prev_trect.data[2] ||
                        ty < prev_trect.data[1] || ty > prev_trect.data[3])
                    {
                        LGridTightCell new_tcell = new LGridTightCell { next = tight_row[tx], lcell = cell_idx };
                        tight_row[tx] = grid.tight.cells.insert(new_tcell);
                    }
                }
            }
        }
    }
}

private static void grid_optimize(LGrid grid)
{
    FreeList<LGridElt> new_elts = new FreeList<LGridElt>();
    new_elts.reserve(grid.num_elts);
    for(int c = 0; c < grid.loose.num_cells; ++c)
    {
        SmallList<int> new_elt_idxs = new SmallList<int>();
        LGridLooseCell lcell = grid.loose.cells[c];
        while(lcell.head != -1)
        {
            LGridElt elt = grid.elts[lcell.head];
            new_elt_idxs.push_back(new_elts.insert(elt));
            lcell.head = elt.next;
        }
        for(int j = 0; j < new_elt_idxs.size(); ++j)
        {
            int new_elt_idx = new_elt_idxs[j];
            new_elts[new_elt_idx].next = lcell.head;
            lcell.head = new_elt_idx;
        }
    }

    grid.elts.swap(new_elts);
}

private static float max_flt(float a, float b)
{
    return Math.Max(a, b);
}

private static int max_int(int a, int b)
{
    a -= b;
    a &= (~a) >> 31;
    return a + b;
}

private static float min_flt(float a, float b)
{
    return Math.Min(a, b);
}

private static int min_int(int a, int b)
{
    a -= b;
    a &= a >> 31;
    return a + b;
}

private static int to_cell_idx(float val, float inv_cell_size, int num_cells)
{
    int cell_pos = (int)(val * inv_cell_size);
    return min_int(max_int(cell_pos, 0), num_cells - 1);
}

private static SimdVec4i to_tcell_idx4(LGrid grid, __m128 rect)
{
    __m128 inv_cell_size_vec = simd_create4f(grid.tight.inv_cell_w, grid.tight.inv_cell_h,
                                                        grid.tight.inv_cell_w, grid.tight.inv_cell_h);
    __m128 cell_xyf_vec = simd_mul4f(rect, inv_cell_size_vec);
    __m128i clamp_vec = simd_create4i(grid.tight.num_cols - 1, grid.tight.num_rows - 1,
                                                 grid.tight.num_cols - 1, grid.tight.num_rows - 1);
    __m128i cell_xy_vec = simd_clamp4i(simd_ftoi4f(cell_xyf_vec), simd_zero4i(), clamp_vec);
    return simd_store4i(cell_xy_vec);
}

private bool lgrid_in_bounds(LGrid grid, float mx, float my, float hx, float hy)
{
    mx -= grid.x;
    my -= grid.y;
    float x1 = mx - hx;
    float y1 = my - hy;
    float x2 = mx + hx;
    float y2 = my + hy;
    return x1 >= 0.0f && x2 < grid.w && y1 >= 0.0f && y2 < grid.h;
}

private void lgrid_move(LGrid grid, int id, float prev_mx, float prev_my, float mx, float my)
{
    int prev_cell_idx = lgrid_lcell_idx(grid, prev_mx, prev_my);
    int new_cell_idx = lgrid_lcell_idx(grid, mx, my);
    LGridLooseCell lcell = grid.loose.cells[prev_cell_idx];
    if(prev_cell_idx == new_cell_idx)
    {
        int elt_idx = lcell.head;
        while(grid.elts[elt_idx].id != id)
            elt_idx = grid.elts[elt_idx].next;

        mx -= grid.x;
        my -= grid.y;
        grid.elts[elt_idx].mx = mx;
        grid.elts[elt_idx].my = my;
        expand_aabb(grid, prev_cell_idx, mx, my, grid.elts[elt_idx].hx, grid.elts[elt_idx].hy);
    }
    else
    {
        int link = lcell.head;
        while(grid.elts[link].id != id)
            link = grid.elts[link].next;
        int elt_idx = link;
        float hx = grid.elts[elt_idx].hx;
        float hy = grid.elts[elt_idx].hy;

        link = grid.elts[elt_idx].next;
        grid.elts.RemoveAt(elt_idx);
        grid.num_elts--;

        lgrid_insert(grid, id, mx, my, hx, hy);
    }
}

private void lgrid_optimize(LGrid grid)
{
    for(int j = 0; j < grid.tight.num_cells; ++j)
        grid.tight.heads[j] = -1;
    grid.tight.cells.Clear();

    grid_optimize(grid);
    for(int c = 0; c < grid.loose.num_cells; ++c)
    {
        LGridLooseCell lcell = grid.loose.cells[c];
        lcell.rect[0] = float.MaxValue;
        lcell.rect[1] = float.MaxValue;
        lcell.rect[2] = -float.MaxValue;
        lcell.rect[3] = -float.MaxValue;

        int elt_idx = lcell.head;
        while(elt_idx != -1)
        {
            LGridElt elt = grid.elts[elt_idx];
            lcell.rect[0] = Math.Min(lcell.rect[0], elt.mx - elt.hx);
            lcell.rect[1] = Math.Min(lcell.rect[1], elt.my - elt.hy);
            lcell.rect[2] = Math.Max(lcell.rect[2], elt.mx + elt.hx);
            lcell.rect[3] = Math.Max(lcell.rect[3], elt.my + elt.hy);
            elt_idx = elt.next;
        }
    }
    for(int c = 0; c < grid.loose.num_cells; ++c)
    {
        LGridLooseCell lcell = grid.loose.cells[c];
        int[] trect = to_tcell_idx4(grid, simd_loadu4f(lcell.rect));
        for(int ty = trect[1]; ty <= trect[3]; ++ty)
        {
            int[] tight_row = grid.tight.heads + ty * grid.tight.num_cols;
            for(int tx = trect[0]; tx <= trect[2]; ++tx)
            {
                LGridTightCell new_tcell = new LGridTightCell(tight_row[tx], c);
                tight_row[tx] = grid.tight.cells.Add(new_tcell);
            }
        }
    }
}

private List<int> lgrid_query(LGrid grid, float mx, float my, float hx, float hy, int omit_id)
{
    mx -= grid.x;
    my -= grid.y;

    float x1 = mx - hx;
    float y1 = my - hy;
    float x2 = mx + hx;
    float y2 = my + hy;

    List<int> lcell_idxs = new List<int>();
    for(int ty = (int)y1; ty <= (int)y2; ++ty)
    {
        int[] tight_row = grid.tight.heads + ty * grid.tight.num_cols;
        for(int tx = (int)x1; tx <= (int)x2; ++tx)
        {
            int tcell_idx = tight_row[tx];
            while(tcell_idx != -1)
            {
                LGridTightCell tcell = grid.tight.cells[tcell_idx];
                LGridLooseCell lcell = grid.loose.cells[tcell.lcell];
                if(!lcell_idxs.Contains(tcell.lcell) && simd_rect_intersect4f(qrect_vec, simd_loadu4f(lcell.rect)))
                    lcell_idxs.Add(tcell.lcell);
                tcell_idx = tcell.next;
            }
        }
    }

    List<int> res = new List<int>();
    for(int j = 0; j < lcell_idxs.Count; ++j)
    {
        LGridLooseCell lcell = grid.loose.cells[lcell_idxs[j]];
        int elt_idx = lcell.head;
        while(elt_idx != -1)
        {
            LGridElt elt = grid.elts[elt_idx];
            if(elt.id != omit_id && simd_rect_intersect4f(qrect_vec, element_rect(elt)))
                res.Add(elt.id);
            elt_idx = elt.next;
        }
    }
    return res;
}

private LGridQuery4 lgrid_query4(LGrid grid, SimdVec4f mx4, SimdVec4f my4, SimdVec4f hx4, SimdVec4f hy4, SimdVec4i omit_id4)
{
    float hx_vec = hx4;
    float hy_vec = hy4;
    float mx_vec = mx4 - grid.x;
    float my_vec = my4 - grid.y;
    float ql_vec = mx_vec - hx_vec;
    float qt_vec = my_vec - hy_vec;
    float qr_vec = mx_vec + hx_vec;
    float qb_vec = my_vec + hy_vec;
    int inv_cell_w_vec = grid.tight.inv_cell_w;
    int inv_cell_h_vec = grid.tight.inv_cell_h;
    int max_x_vec = grid.tight.num_cols - 1;
    int max_y_vec = grid.tight.num_rows - 1;
    int tmin_x_vec = simd_clamp4i(simd_ftoi4f(simd_mul4f(ql_vec, inv_cell_w_vec)), simd_zero4i(), max_x_vec);
    int tmin_y_vec = simd_clamp4i(simd_ftoi4f(simd_mul4f(qt_vec, inv_cell_h_vec)), simd_zero4i(), max_y_vec);
    int tmax_x_vec = simd_clamp4i(simd_ftoi4f(simd_mul4f(qr_vec, inv_cell_w_vec)), simd_zero4i(), max_x_vec);
    int tmax_y_vec = simd_clamp4i(simd_ftoi4f(simd_mul4f(qb_vec, inv_cell_h_vec)), simd_zero4i(), max_y_vec);
    int[] tmin_x4 = tmin_x_vec;
    int[] tmin_y4 = tmin_y_vec;
    int[] tmax_x4 = tmax_x_vec;
    int[] tmax_y4 = tmax_y_vec;
    float[] ql4 = ql_vec;
    float[] qt4 = qt_vec;
    float[] qr4 = qr_vec;
    float[] qb4 = qb_vec;
    LGridQuery4 res4;
    for(int k = 0; k < 4; ++k)
    {
        int[] trect = { tmin_x4[k], tmin_y4[k], tmax_x4[k], tmax_y4[k] };
        int omit_id = omit_id4[k];

        List<int> lcell_idxs = new List<int>();
        float[] qrect_vec = { ql4[k], qt4[k], qr4[k], qb4[k] };
        for(int ty = trect[1]; ty <= trect[3]; ++ty)
        {
            int[] tight_row = grid.tight.heads + ty * grid.tight.num_cols;
            for(int tx = trect[0]; tx <= trect[2]; ++tx)
            {
                int tcell_idx = tight_row[tx];
                while(tcell_idx != -1)
                {
                    LGridTightCell tcell = grid.tight.cells[tcell_idx];
                    if(!lcell_idxs.Contains(tcell.lcell) && simd_rect_intersect4f(qrect_vec, simd_loadu4f(grid.loose.cells[tcell.lcell].rect)))
                        lcell_idxs.Add(tcell.lcell);
                    tcell_idx = tcell.next;
                }
            }
        }

        for(int j = 0; j < lcell_idxs.Count; ++j)
        {
            LGridLooseCell lcell = grid.loose.cells[lcell_idxs[j]];
            int elt_idx = lcell.head;
            while(elt_idx != -1)
            {
                LGridElt elt = grid.elts[elt_idx];
                if(elt.id != omit_id && simd_rect_intersect4f(qrect_vec, element_rect(elt)))
                    res4.elements[k].Add(elt.id);
                elt_idx = elt.next;
            }
        }
    }
    return res4;
}

private void lgrid_remove(LGrid grid, int id, float mx, float my)
{
    LGridLooseCell lcell = grid.loose.cells[lgrid_lcell_idx(grid, mx, my)];
    int link = lcell.head;
    while(grid.elts[link].id != id)
        link = grid.elts[link].next;

    int elt_idx = link;
    link = grid.elts[elt_idx].next;
    grid.elts.RemoveAt(elt_idx);
    grid.num_elts--;
}
    }

    public class SmallList<T>
{
    private const int fixed_cap = 256;
    private ListData ld;

    public SmallList()
    {
    }

    public SmallList(SmallList<T> other)
    {
        if(other.ld.cap == fixed_cap)
        {
            ld = other.ld;
            ld.data = ld.buf;
        }
        else
        {
            reserve(other.ld.num);
            for(int j = 0; j < other.size(); ++j)
                ld.data[j] = other.ld.data[j];
            ld.num = other.ld.num;
            ld.cap = other.ld.cap;
        }
    }

    ~SmallList()
    {
        if(ld.data != ld.buf)
            Array.Clear(ld.data, 0, ld.data.Length);
    }

    public T this[int n]
    {
        get
        {
            if(n >= 0 && n < ld.num)
                return ld.data[n];
            throw new IndexOutOfRangeException();
        }
        set
        {
            if(n >= 0 && n < ld.num)
                ld.data[n] = value;
            else
                throw new IndexOutOfRangeException();
        }
    }

    public void clear()
    {
        ld.num = 0;
    }

    public T[] data()
    {
        return ld.data;
    }

    public int find_index(T element)
    {
        for(int j = 0; j < ld.num; ++j)
        {
            if(ld.data[j].Equals(element))
                return j;
        }
        return -1;
    }

    public SmallList<T> operator =(SmallList<T> other)
    {
        new SmallList<T>(other).swap(this);
        return this;
    }

    public T pop_back()
    {
        return ld.data[--ld.num];
    }

    public void push_back(T element)
    {
        if(ld.num >= ld.cap)
            reserve(ld.cap * 2);
        ld.data[ld.num++] = element;
    }

    public void reserve(int n)
    {
        if(n > ld.cap)
        {
            if(ld.cap == fixed_cap)
            {
                ld.data = new T[n];
                Array.Copy(ld.buf, ld.data, ld.buf.Length);
            }
            else
            {
                Array.Resize(ref ld.data, n);
            }
            ld.cap = n;
        }
    }

    public int size()
    {
        return ld.num;
    }

    public void swap(SmallList<T> other)
    {
        ListData ld1 = ld;
        ListData ld2 = other.ld;
        bool use_fixed1 = ld1.data == ld1.buf;
        bool use_fixed2 = ld2.data == ld2.buf;
        ListData temp = ld1;
        ld1 = ld2;
        ld2 = temp;
        if(use_fixed1)
            ld2.data = ld2.buf;
        if(use_fixed2)
            ld1.data = ld1.buf;
    }

    private struct ListData
    {
        public T[] buf;

        public int cap;

        public T[] data;

        public int num;

        public ListData()
        {
            buf = new T[fixed_cap];
            data = buf;
            num = 0;
            cap = fixed_cap;
        }
    }
}
}
