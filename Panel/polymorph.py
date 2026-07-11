import struct
import os
import random
import string
import re


def _r16(buf, off):
    return struct.unpack_from('<H', buf, off)[0]


def _r32(buf, off):
    return struct.unpack_from('<I', buf, off)[0]


def _w32(buf, off, val):
    struct.pack_into('<I', buf, off, val)


def _rand_ident(length):
    first = random.choice(string.ascii_letters)
    rest = ''.join(random.choices(string.ascii_letters + string.digits, k=max(0, length - 1)))
    return first + rest


def _parse_pe(buf):
    if buf[0:2] != b'MZ':
        raise ValueError("Not PE")

    pe_off = _r32(buf, 0x3C)
    if buf[pe_off:pe_off + 4] != b'PE\x00\x00':
        raise ValueError("Bad PE sig")

    coff = pe_off + 4
    ts_off = coff + 4
    num_sect = _r16(buf, coff + 2)
    opt_sz = _r16(buf, coff + 16)

    opt = coff + 20
    magic = _r16(buf, opt)
    if magic == 0x20b:
        dd_off = opt + 112
    else:
        dd_off = opt + 96

    sections = []
    sec_start = opt + opt_sz
    for i in range(num_sect):
        s = sec_start + i * 40
        sections.append({
            'va': _r32(buf, s + 12),
            'vs': _r32(buf, s + 8),
            'rp': _r32(buf, s + 20),
            'rs': _r32(buf, s + 16),
        })

    def rva2off(rva):
        for sec in sections:
            if sec['va'] <= rva < sec['va'] + sec['rs']:
                return sec['rp'] + (rva - sec['va'])
        return None

    cli_rva = _r32(buf, dd_off + 14 * 8)
    cli_off = rva2off(cli_rva)
    if cli_off is None:
        raise ValueError("CLI header not found")

    meta_rva = _r32(buf, cli_off + 8)
    meta_off = rva2off(meta_rva)
    if meta_off is None:
        raise ValueError("Metadata not found")

    sig = _r32(buf, meta_off)
    if sig != 0x424A5342:
        raise ValueError("Bad metadata sig")

    ver_len = _r32(buf, meta_off + 12)
    pos = meta_off + 16 + ver_len
    pos = (pos + 3) & ~3

    num_streams = _r16(buf, pos + 2)
    pos += 4

    streams = {}
    for _ in range(num_streams):
        s_off = _r32(buf, pos)
        s_sz = _r32(buf, pos + 4)
        pos += 8
        name_start = pos
        while buf[pos] != 0:
            pos += 1
        s_name = buf[name_start:pos].decode('ascii')
        pos += 1
        pos = (pos + 3) & ~3
        streams[s_name] = {'offset': meta_off + s_off, 'size': s_sz}

    return {
        'pe_off': pe_off,
        'ts_off': ts_off,
        'sections': sections,
        'streams': streams,
        'rva2off': rva2off,
    }


def _get_root_namespace(source_dir):
    if not source_dir or not os.path.isdir(source_dir):
        return None

    roots = set()
    for root, dirs, files in os.walk(source_dir):
        for f in files:
            if not f.endswith('.cs'):
                continue
            try:
                with open(os.path.join(root, f), 'r', encoding='utf-8-sig') as fh:
                    for m in re.finditer(r'namespace\s+([\w.]+)', fh.read()):
                        roots.add(m.group(1).split('.')[0])
            except:
                continue

    for r in roots:
        if len(r) > 2:
            return r
    return None


def _collect_stream_ranges(pe):
    ranges = []
    for name in ('#Strings', '#US', '#Blob', '#GUID', '#~'):
        if name in pe['streams']:
            s = pe['streams'][name]
            ranges.append((s['offset'], s['offset'] + s['size']))
    return ranges


def _in_ranges(idx, length, ranges):
    for start, end in ranges:
        if start <= idx and idx + length <= end:
            return True
    return False


def mutate(data, source_dir=None):
    buf = bytearray(data)

    try:
        pe = _parse_pe(buf)
    except Exception:
        return buf

    ts = random.randint(0x50000000, 0x68000000)
    _w32(buf, pe['ts_off'], ts)

    if '#GUID' in pe['streams']:
        g = pe['streams']['#GUID']
        for i in range(16):
            buf[g['offset'] + i] = random.randint(0, 255)

    root_ns = _get_root_namespace(source_dir)
    if root_ns and len(root_ns) >= 3:
        rep = _rand_ident(len(root_ns))
        old_u8 = root_ns.encode('utf-8')
        new_u8 = rep.encode('utf-8')
        old_u16 = root_ns.encode('utf-16-le')
        new_u16 = rep.encode('utf-16-le')

        meta_ranges = _collect_stream_ranges(pe)

        pos = 0
        while pos < len(buf):
            idx = buf.find(old_u8, pos)
            if idx == -1:
                break
            if _in_ranges(idx, len(old_u8), meta_ranges):
                buf[idx:idx + len(old_u8)] = new_u8
            pos = idx + len(old_u8)

        pos = 0
        while pos < len(buf):
            idx = buf.find(old_u16, pos)
            if idx == -1:
                break
            if _in_ranges(idx, len(old_u16), meta_ranges):
                buf[idx:idx + len(old_u16)] = new_u16
            pos = idx + len(old_u16)

    return bytes(buf)
